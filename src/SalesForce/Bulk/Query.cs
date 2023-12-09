// Copyright 2023, Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkQuery",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Query : AggregateOperationBase {

		#region .ctor
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
			"soql",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Soql {
			get;
			set;
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

		public sealed override void PerformWork( Pair<LoginResponse, IStep> jobProcess ) {
			var step = jobProcess.Second ?? throw new System.ArgumentException();
			var workOrder = step.WorkOrder;
			var loginResponse = jobProcess.First;

			var jobResponse = this.CreateJob( loginResponse );
			var id = jobResponse.Id;

			jobResponse = this.WaitUntilStateOption(
				this.QueryJob( loginResponse, id ), loginResponse, id,
				new StateOption[ 3 ] { StateOption.Aborted, StateOption.Failed, StateOption.JobComplete }
			);

			SelectResult result;
			System.String locator = null;
			do {
				result = this.GetResults( loginResponse, id, locator, ColumnDelimiterOption.FromName( jobResponse.ColumnDelimiter ).Value, LineEndingOption.FromName( jobResponse.LineEnding ).Value );
				this.WriteRecords( workOrder, result );
				locator = result.Locator;
			} while ( !System.String.IsNullOrEmpty( locator ) );

			this.DeleteJob( loginResponse, id );
		}

		private SelectResult GetResults( LoginResponse loginResponse, System.String id, System.String locator, System.Char columnDelimiter, System.String lineEnding ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var urib = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id + "/results" ) {
				Query = System.String.IsNullOrEmpty( locator )
					? System.String.Format( "maxRecords={1}", locator, this.BatchSize )
					: System.String.Format( "locator={0}&maxRecords={1}", locator, this.BatchSize )
			};
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
