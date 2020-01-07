using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkSelect",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class BulkSelect : BulkOperationBase, Icod.Wod.Data.ITableSource {

		#region .ctor
		public BulkSelect() : base() {
		}
		public BulkSelect( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"soql",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Soql {
			get;
			set;
		}

		[System.Xml.Serialization.XmlElement(
			"destination",
			Type = typeof( DbDestination ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public DbDestination Destination {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var dest = this.Destination;
			if ( null == dest ) {
				throw new System.InvalidOperationException();
			}
			dest.WriteRecords( workOrder, this );
		}
		public IEnumerable<DataTable> ReadTables( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var credential = Credential.GetCredential( this.InstanceName, workOrder );
			var loginToken = new Login( workOrder ).GetLoginResponse( credential, System.Text.Encoding.UTF8 );
			var job = this.CreateSelect( loginToken, this.Soql );
			if ( !(
					"JobComplete".Equals( job.State, System.StringComparison.OrdinalIgnoreCase )
					|| "UploadComplete".Equals( job.State, System.StringComparison.OrdinalIgnoreCase )
					|| "InProgress".Equals( job.State, System.StringComparison.OrdinalIgnoreCase )
			) ) {
				var ex = new System.InvalidOperationException( "The bulk API job is in an unknown state: " + job.State ?? System.String.Empty );
				ex.Data.Add( "Soql", this.Soql );
				ex.Data.Add( "bulkOperation", job );
				throw ex;
			}
			var wait = ( this.Wait ?? new Wait() );
			var sleepTime = wait.Initial;
			System.Threading.Thread.Sleep( sleepTime );
			if ( !"JobComplete".Equals( job.State, System.StringComparison.OrdinalIgnoreCase ) ) {
				job = this.QueryJob( loginToken, job.Id );
			}
			sleepTime = wait.Minimum;
			var increment = wait.Increment;
			var maximum = wait.Maximum;
			System.String locator = null;
			var result = new SelectResult();
			do {
#if TRACE
				System.Console.Error.WriteLine( "{0} : {1}", job.Id, job.State );
#endif
				if ( "JobComplete".Equals( job.State, System.StringComparison.OrdinalIgnoreCase ) ) {
					result = this.GetResults( loginToken, job.Id, result.Locator );
					yield return result.ReadFile();
					if ( System.String.IsNullOrEmpty( result.Locator ) ) {
						this.DeleteJob( loginToken, job.Id );
						break;
					}
				} else if (
					"UploadComplete".Equals( job.State, System.StringComparison.OrdinalIgnoreCase )
					|| "InProgress".Equals( job.State, System.StringComparison.OrdinalIgnoreCase )
				) {
					System.Threading.Thread.Sleep( sleepTime );
					if ( sleepTime < maximum ) {
						sleepTime = System.Math.Min( sleepTime + increment, maximum );
					}
				} else {
					var ex = new System.InvalidOperationException( "The bulk API job is in an unknown state: " + job.State ?? System.String.Empty );
					ex.Data.Add( "Soql", this.Soql );
					ex.Data.Add( "bulkOperation", job );
					throw ex;
				}
				job = this.QueryJob( loginToken, job.Id );
			} while ( true );

			yield break;
		}

		protected JobResponse CreateSelect( LoginResponse loginToken, System.String soql ) {
			var instanceUrl = new System.Uri( loginToken.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() ).Uri;
			var request = this.GetHttpWebRequest( loginToken, uri );
			request.ContentType = "application/json; charset=utf-8";
			request.Method = System.Net.Http.HttpMethod.Post.Method;
			using ( var w = request.GetRequestStream() ) {
				var jr = new {
					operation = "query",
					query = this.Soql,
					contentType = "CSV",
					columnDelimiter = "COMMA",
					lineEnding = "CRLF"
				};
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( jr );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			var response = (System.Net.HttpWebResponse)request.GetResponse();
			var sc = response.StatusCode;
			var failure = (
				( System.Net.HttpStatusCode.OK != sc )
				&& ( System.Net.HttpStatusCode.Created != sc )
			);
			if ( failure ) {
				throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
			}
			return this.GetJobResponse( response );
		}

		private SelectResult GetResults( LoginResponse loginToken, System.String id, System.String locator ) {
			var instanceUrl = new System.Uri( loginToken.InstanceUrl );
			var urib = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id + "/results" );
			urib.Query = System.String.IsNullOrEmpty( locator )
				? System.String.Format( "maxRecords={1}", locator, this.BatchSize )
				: System.String.Format( "locator={0}&maxRecords={1}", locator, this.BatchSize )
			;
			var uri = urib.Uri;
			var request = this.GetHttpWebRequest( loginToken, uri );
			request.Method = System.Net.Http.HttpMethod.Get.Method;
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			var response = request.GetResponse();
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var source = response.GetResponseStream() ) {
					source.CopyTo( buffer );
				}
				return new SelectResult {
					RecordCount = System.Convert.ToInt32( response.Headers[ "Sforce-NumberOfRecords" ] ),
					Locator = response.Headers[ "Sforce-Locator" ],
					Body = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" )
				};
			}
		}
		#endregion methods

	}

}