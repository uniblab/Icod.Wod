using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkQuery",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Query : AggregateOperationBase {

		#region  fields
		public const System.Int32 DefaultBatchSize = 10000;

		public static readonly System.Decimal DefaultApiVersion;
		#endregion  fields


		#region .ctor
		static Query() {
			DefaultApiVersion = new System.Decimal( 47.0 );
		}

		public Query() : base() {
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		public Query( WorkOrder workOrder ) : base( workOrder ) {
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		public Query( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		public Query( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"apiVersion",
			Namespace = "http://Icod.Wod"
		)]
		public sealed override System.Decimal ApiVersion {
			get {
				return base.ApiVersion;
			}
			set {
				base.ApiVersion = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"soql",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Soql {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"batchSize",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultBatchSize )]
		public sealed override System.Int32 BatchSize {
			get {
				return base.BatchSize;
			}
			set {
				base.BatchSize = value;
			}
		}
		#endregion properties


		#region methods
		protected sealed override System.String GetServicePath() {
			return System.String.Format( "/services/data/v{0:F1}/jobs/query", this.ApiVersion );
		}

		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
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
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				var failure = !(
					( System.Net.HttpStatusCode.OK == sc )
					|| ( System.Net.HttpStatusCode.Created == sc )
				);
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}

		public sealed override void PerformWork( JobProcess jobProcess ) {
			var step = jobProcess.Step;
			if ( null == step ) {
				throw new System.ArgumentException();
			}
			var workOrder = step.WorkOrder;
			var loginResponse = jobProcess.LoginResponse;
			var semaphore = jobProcess.Semaphore;

			semaphore.Wait();
			var jobResponse = this.CreateJob( loginResponse );
			semaphore.Release();
			var id = jobResponse.Id;

			var wait = ( this.Wait ?? new Wait() );
			var sleepTime = wait.Initial;
			if ( 0 < sleepTime ) {
				System.Threading.Thread.Sleep( sleepTime );
			}
			sleepTime = wait.Minimum;
			var max = wait.Maximum;
			var state = jobResponse.state;
			while (
				!StateOption.JobComplete.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
				&& !StateOption.Aborted.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
				&& !StateOption.Failed.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
				&& ( sleepTime < max )
			) {
				System.Threading.Thread.Sleep( sleepTime );
				sleepTime = System.Math.Min( max, sleepTime + wait.Increment );
				semaphore.Wait();
				jobResponse = this.QueryJob( loginResponse, id );
				semaphore.Release();
				state = jobResponse.state;
			}
			while (
				!StateOption.JobComplete.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
				&& !StateOption.Aborted.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
				&& !StateOption.Failed.Value.Equals( state, System.StringComparison.OrdinalIgnoreCase )
			) {
				System.Threading.Thread.Sleep( max );
				semaphore.Wait();
				jobResponse = this.QueryJob( loginResponse, id );
				semaphore.Release();
				state = jobResponse.state;
			}

			SelectResult result = null;
			System.String locator = null;
			do {
				semaphore.Wait();
				result = this.GetResults( loginResponse, id, locator, ColumnDelimiterOption.FromName( jobResponse.columnDelimiter ).Value, LineEndingOption.FromName( jobResponse.lineEnding ).Value );
				semaphore.Release();
				this.WriteRecords( workOrder, result );
				locator = result.Locator;
			} while ( !System.String.IsNullOrEmpty( locator ) );

			semaphore.Wait();
			this.DeleteJob( loginResponse, id );
			semaphore.Release();
		}

		private SelectResult GetResults( LoginResponse loginResponse, System.String id, System.String locator, System.Char columnDelimiter, System.String lineEnding ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var urib = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id + "/results" );
			urib.Query = System.String.IsNullOrEmpty( locator )
				? System.String.Format( "maxRecords={1}", locator, this.BatchSize )
				: System.String.Format( "locator={0}&maxRecords={1}", locator, this.BatchSize )
			;
			var uri = urib.Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			request.Method = System.Net.Http.HttpMethod.Get.Method;
			using ( var response = request.GetResponse() ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var source = response.GetResponseStream() ) {
						source.CopyTo( buffer );
					}
					return new SelectResult {
						RecordCount = System.Convert.ToInt32( response.Headers[ "Sforce-NumberOfRecords" ] ),
						Locator = response.Headers[ "Sforce-Locator" ],
						Body = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" ),
						ColumnDelimiter = columnDelimiter,
						LineEnding = lineEnding
					};
				}
			}
		}
		#endregion methods

	}

}