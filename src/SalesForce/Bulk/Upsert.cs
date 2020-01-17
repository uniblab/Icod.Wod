using System.Linq;
using System.Net;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkUpsert",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Upsert : AggregateMutationOperationBase {

		#region .ctor
		public Upsert() : base() {
		}
		public Upsert( WorkOrder workOrder ) : base( workOrder ) {
		}
		public Upsert( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
		}
		public Upsert( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"externalIdFieldName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String ExternalIdFieldName {
			get;
			set;
		}
		#endregion properties


		#region methods
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
					operation = "upsert",
					externalIdFieldName = this.ExternalIdFieldName,
					columnDelimiter = "COMMA",
					contentType = "CSV",
					lineEnding = "CRLF",
					@object = this.Object,
				};
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( jr );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				if ( System.Net.HttpStatusCode.OK != sc ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse, System.String operation ) {
			return this.CreateJob( loginResponse );
		}
		public sealed override void PerformWork( JobProcess jobProcess ) {
			var loginResponse = jobProcess.LoginResponse;
			var semaphore = jobProcess.Semaphore;

			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}