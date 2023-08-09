// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public abstract class AggregateOperationBase : Data.DbIODescriptorBase, IAggregateOperation {

		#region fields
		public const System.Int32 DefaultBatchSize = 10000;
		public static readonly System.Decimal DefaultApiVersion;
		#endregion fields


		#region  .ctor
		static AggregateOperationBase() {
			DefaultApiVersion = new System.Decimal( 47 );
		}

		protected AggregateOperationBase() : base() {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateOperationBase( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateOperationBase( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Ignore;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Ignore;
			this.BatchSize = DefaultBatchSize;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"apiVersion",
			Namespace = "http://Icod.Wod"
		)]
		public virtual System.Decimal ApiVersion {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"batchSize",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultBatchSize )]
		public virtual System.Int32 BatchSize {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Ignore )]
		public override System.Data.MissingSchemaAction MissingSchemaAction {
			get {
				return base.MissingSchemaAction;
			}
			set {
				base.MissingSchemaAction = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"missingMappingAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Ignore )]
		public override System.Data.MissingMappingAction MissingMappingAction {
			get {
				return base.MissingMappingAction;
			}
			set {
				base.MissingMappingAction = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"tag",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public virtual System.String Tag {
			get;
			set;
		}

		[System.Xml.Serialization.XmlElement(
			"wait",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		[System.ComponentModel.DefaultValue( null )]
		public Wait Wait {
			get;
			set;
		}
		#endregion properties


		#region methods
		public abstract void PerformWork( Pair<LoginResponse, IStep> jobProcess );

		protected abstract System.String GetServicePath();
		protected abstract JobResponse CreateJob( LoginResponse loginResponse );
		protected virtual JobResponse GetJobResponse( System.Net.HttpWebResponse response ) {
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var source = response.GetResponseStream() ) {
					source.CopyTo( buffer );
				}
				var json = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" );
				return Newtonsoft.Json.JsonConvert.DeserializeObject<JobResponse>( json );
			}
		}
		protected virtual void DeleteJob( LoginResponse loginResponse, System.String id ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
			request.Method = System.Net.Http.HttpMethod.Delete.Method;
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var failure = System.Net.HttpStatusCode.NoContent != response.StatusCode;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
				}
			}
		}
		protected virtual JobResponse AbortJob( LoginResponse loginResponse, System.String id ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			request.ContentType = "application/json; charset=utf-8";
			request.Method = "PATCH";
			using ( var w = request.GetRequestStream() ) {
				var jr = new {
					state = "Aborted"
				};
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( jr );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				var failure = System.Net.HttpStatusCode.OK != response.StatusCode;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}

		protected virtual JobResponse QueryJob( LoginResponse loginResponse, System.String id ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
			request.Method = System.Net.Http.HttpMethod.Get.Method;
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var failure = System.Net.HttpStatusCode.OK != response.StatusCode;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}

		protected JobResponse SendRequest( LoginResponse loginResponse, System.String id, System.String method, JobRequest data ) {
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
			request.Method = method;
			using ( var w = request.GetRequestStream() ) {
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( data );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				return this.GetJobResponse( response );
			}
		}
		#endregion methods

	}

}
