using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	public abstract class BulkOperationBase : SFOperationBase, Icod.Wod.IStep {

		#region fields
		private const System.Int32 DefaultBatchSize = 10000;

		private System.Int32 myBatchSize;
		#endregion fields


		#region .ctor
		protected BulkOperationBase() : base() {
			myBatchSize = DefaultBatchSize;
			this.Wait = new Wait();
		}
		protected BulkOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			myBatchSize = DefaultBatchSize;
			this.Wait = new Wait();
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"batchSize",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultBatchSize )]
		public System.Int32 BatchSize {
			get {
				return myBatchSize;
			}
			set {
				myBatchSize = value;
			}
		}

		[System.Xml.Serialization.XmlElement(
			"wait",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public Wait Wait {
			get;
			set;
		}
		#endregion properties


		#region methods
		public abstract void DoWork( WorkOrder workOrder );


		protected virtual System.String GetServicePath() {
			return System.String.Format( "/services/data/v{0:F1}/jobs/query", this.ApiVersion );
		}

		protected void DeleteJob( LoginResponse loginToken, System.String id ) {
			var instanceUrl = new System.Uri( loginToken.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginToken.AccessToken );
			request.Method = System.Net.Http.HttpMethod.Delete.Method;
			var response = (System.Net.HttpWebResponse)request.GetResponse();
			var failure = System.Net.HttpStatusCode.NoContent != response.StatusCode;
			if ( failure ) {
				throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
			}
		}
		protected JobResponse QueryJob( LoginResponse loginToken, System.String id ) {
			var instanceUrl = new System.Uri( loginToken.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginToken.AccessToken );
			request.Method = System.Net.Http.HttpMethod.Get.Method;
			var response = (System.Net.HttpWebResponse)request.GetResponse();
			var failure = System.Net.HttpStatusCode.OK != response.StatusCode;
			if ( failure ) {
				throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
			}
			return this.GetJobResponse( response );
		}
		protected JobResponse CloseJob( LoginResponse loginToken, System.String id ) {
			var instanceUrl = new System.Uri( loginToken.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginToken.AccessToken );
			request.Method = "PATCH";
			var response = (System.Net.HttpWebResponse)request.GetResponse();
			var failure = System.Net.HttpStatusCode.OK != response.StatusCode;
			if ( failure ) {
				throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
			}
			return this.GetJobResponse( response );
		}

		protected JobResponse GetJobResponse( System.Net.HttpWebResponse response ) {
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var source = response.GetResponseStream() ) {
					source.CopyTo( buffer );
				}
				var json = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" );
				return Newtonsoft.Json.JsonConvert.DeserializeObject<JobResponse>( json );
			}
		}
		protected System.Net.HttpWebRequest GetHttpWebRequest( LoginResponse loginToken, System.Uri uri ) {
			if ( null == loginToken ) {
				throw new System.ArgumentNullException( "loginToken" );
			}

			var output = System.Net.WebRequest.CreateHttp( uri );
			output.Headers.Add( "Authorization", "Bearer " + loginToken.AccessToken );
#if DEBUG
			output.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			output.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			return output;
		}
		#endregion methods

	}

}