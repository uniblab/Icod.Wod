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
			var request = this.BuildSalesForceRequest( loginResponse, id, "application/json; charset=utf-8", "DELETE" );
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var failure = System.Net.HttpStatusCode.NoContent != response.StatusCode;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
				}
			}
		}
		protected virtual JobResponse AbortJob( LoginResponse loginResponse, System.String id ) {
			var request = this.BuildSalesForceRequest( loginResponse, id, "application/json; charset=utf-8", "PATCH" );
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
			var request = this.BuildSalesForceRequest( loginResponse, id, "application/json; charset=utf-8", "GET" );
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var failure = System.Net.HttpStatusCode.OK != response.StatusCode;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode.ToString(), response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}

		protected System.Net.HttpWebRequest BuildSalesForceRequest(
			LoginResponse loginResponse, System.String contentType, System.String method
		) {
			return this.BuildSalesForceRequest(
				loginResponse, null, contentType, method, null, null, null 
			);
		}
		protected System.Net.HttpWebRequest BuildSalesForceRequest(
			LoginResponse loginResponse, System.String id,
			System.String contentType, System.String method
		) {
			return this.BuildSalesForceRequest(
				loginResponse, id, contentType, method, null, null, null
			);
		}
		protected System.Net.HttpWebRequest BuildSalesForceRequest(
			LoginResponse loginResponse, System.String id,
			System.String contentType, System.String method,
			System.String subPath
		) {
			return this.BuildSalesForceRequest(
				loginResponse, id, contentType, method, subPath, null, null
			);
		}
		protected System.Net.HttpWebRequest BuildSalesForceRequest(
			LoginResponse loginResponse, System.String id,
			System.String contentType, System.String method,
			System.String subPath, System.String contentUrl,
			System.String query
		) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var pathValue = System.String.IsNullOrEmpty( contentUrl )
				? this.GetServicePath()
				: contentUrl
			;
			if ( !System.String.IsNullOrEmpty( id ) ) {
				pathValue += "/" + id;
				if ( !System.String.IsNullOrEmpty( subPath ) ) {
					pathValue += "/" + subPath;
				}
			}
			var urib = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, pathValue ) {
				Query = query
			};

			var request = System.Net.WebRequest.CreateHttp( urib.Uri );
			request.ContentType = contentType;
			request.Method = method.ToUpper();
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			return request;
		}
		protected virtual JobResponse WaitUntilStateOption(
			JobResponse start, LoginResponse loginResponse, System.String id,
			System.Collections.Generic.IEnumerable<StateOption> collection
		) {
			var states = collection.Select( x => x.ToString() );
			if ( states.Contains( start.State ) ) {
				return start;
			}

			var wait = ( this.Wait ?? new Wait() );
			var sleepTime = wait.Initial;
			var max = wait.Maximum;
			var inc = wait.Increment;
			if ( 0 < sleepTime ) {
				System.Threading.Thread.Sleep( sleepTime );
			}
			var jobResponse = this.QueryJob( loginResponse, id );
			sleepTime = wait.Minimum;
			while (
				!states.Contains( jobResponse.State )
				&& ( sleepTime < max )
			) {
				System.Threading.Thread.Sleep( sleepTime );
				sleepTime = System.Math.Min( max, sleepTime + inc );
				jobResponse = this.QueryJob( loginResponse, id );
			}
			while ( !states.Contains( jobResponse.State ) ) {
				System.Threading.Thread.Sleep( max );
				jobResponse = this.QueryJob( loginResponse, id );
			}
			return jobResponse;
		}
		#endregion methods

	}

}
