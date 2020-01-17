using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public abstract class AggregateMutationOperationBase : AggregateOperationBase {

		#region  fields
		public const System.Int32 EOF = -1;
		public const System.Int32 MaxFileSize = 103809024;
		public const System.Int32 DefaultBatchSize = 10000;

		private const System.String FailedResults = "failedResults";
		private const System.String SuccessfulResults = "successfulResults";

		public static readonly System.Decimal DefaultApiVersion;
		#endregion  fields


		#region  .ctor
		static AggregateMutationOperationBase() {
			DefaultApiVersion = new System.Decimal( 41.0 );
		}

		protected AggregateMutationOperationBase() : base() {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateMutationOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateMutationOperationBase( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		protected AggregateMutationOperationBase( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = System.Data.MissingMappingAction.Passthrough;
			this.MissingSchemaAction = System.Data.MissingSchemaAction.Add;
			this.ApiVersion = DefaultApiVersion;
			this.BatchSize = DefaultBatchSize;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.Data.MissingSchemaAction.Add )]
		public sealed override System.Data.MissingSchemaAction MissingSchemaAction {
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
		[System.ComponentModel.DefaultValue( System.Data.MissingMappingAction.Passthrough )]
		public sealed override System.Data.MissingMappingAction MissingMappingAction {
			get {
				return base.MissingMappingAction;
			}
			set {
				base.MissingMappingAction = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"apiVersion",
			Namespace = "http://Icod.Wod"
		)]
		public override System.Decimal ApiVersion {
			get {
				return base.ApiVersion;
			}
			set {
				base.ApiVersion = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"object",
			Namespace = "http://Icod.Wod"
		)]
		public virtual System.String Object {
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

		public DbDestination Success {
			get;
			set;
		}
		public DbDestination Error {
			get;
			set;
		}
		#endregion properties


		#region methods
		public override void PerformWork( JobProcess jobProcess ) {
			var step = jobProcess.Step;
			if ( null == step ) {
				throw new System.ArgumentException();
			}
			var workOrder = step.WorkOrder;
			var loginResponse = jobProcess.LoginResponse;
			var semaphore = jobProcess.Semaphore;

			foreach ( var file in this.BuildFiles(
				this.ReadTables( workOrder ), ColumnDelimiterOption.Comma.Value, LineEndingOption.CRLF.Value, '"', this.BatchSize
			) ) {
				semaphore.Wait();
				var jobResponse = this.CreateJob( loginResponse );
				semaphore.Release();
				var id = jobResponse.Id;

				var wait = ( this.Wait ?? new Wait() );
				var sleepTime = wait.Initial;
				while (
					!StateOption.Open.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
				) {
					System.Threading.Thread.Sleep( sleepTime );
					sleepTime = System.Math.Min( wait.Maximum, sleepTime + wait.Increment );
					semaphore.Wait();
					jobResponse = this.QueryJob( loginResponse, id );
					semaphore.Release();
				}

				semaphore.Wait();
				this.UploadData( loginResponse, jobResponse, file );
				semaphore.Release();

				semaphore.Wait();
				jobResponse = this.QueryJob( loginResponse, id );
				semaphore.Release();

				sleepTime = wait.Initial;
				while (
					!StateOption.Open.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
				) {
					System.Threading.Thread.Sleep( sleepTime );
					sleepTime = System.Math.Min( wait.Maximum, sleepTime + wait.Increment );
					semaphore.Wait();
					jobResponse = this.QueryJob( loginResponse, id );
					semaphore.Release();
				}

				semaphore.Wait();
				this.CloseJob( loginResponse, id );
				semaphore.Release();

				semaphore.Wait();
				jobResponse = this.QueryJob( loginResponse, id );
				semaphore.Release();

				sleepTime = wait.Initial;
				while (
					!StateOption.JobComplete.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
				) {
					System.Threading.Thread.Sleep( sleepTime );
					sleepTime = System.Math.Min( wait.Maximum, sleepTime + wait.Increment );
					semaphore.Wait();
					jobResponse = this.QueryJob( loginResponse, id );
					semaphore.Release();
				}

				if ( null != this.Error ) {
					var error = this.GetResults( loginResponse, jobResponse, FailedResults );
					if ( !System.String.IsNullOrEmpty( error.Body ) ) {
						this.Error.WriteRecords( workOrder, this.GetResults( loginResponse, jobResponse, FailedResults ) );
					}
				}
				if ( null != this.Success ) {
					var success = this.GetResults( loginResponse, jobResponse, FailedResults );
					if ( !System.String.IsNullOrEmpty( success.Body ) ) {
						this.Success.WriteRecords( workOrder, this.GetResults( loginResponse, jobResponse, SuccessfulResults ) );
					}
				}

				semaphore.Wait();
				this.DeleteJob( loginResponse, id );
				semaphore.Release();
			}
		}

		private SelectResult GetResults( LoginResponse loginResponse, JobResponse jobResponse, System.String results ) {
			var id = jobResponse.Id;
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var urib = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, this.GetServicePath() + "/" + id + "/" + results );
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
						Body = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" ),
						ColumnDelimiter = ColumnDelimiterOption.FromName( jobResponse.columnDelimiter ).Value,
						LineEnding = LineEndingOption.FromName( jobResponse.lineEnding ).Value,
					};
				}
			}
		}

		protected virtual JobResponse CreateJob( LoginResponse loginResponse, System.String operation ) {
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
					operation = operation,
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
		protected sealed override System.String GetServicePath() {
			return System.String.Format( "/services/data/v{0:F1}/jobs/ingest", this.ApiVersion );
		}
		protected void CloseJob( LoginResponse loginResponse, System.String id ) {
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
					state = "UploadComplete"
				};
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( jr );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				var failure = System.Net.HttpStatusCode.OK != sc;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
				}
			}
		}

		protected virtual void UploadData( LoginResponse loginResponse, JobResponse jobResponse, System.String data ) {
			var instanceUrl = new System.Uri( loginResponse.InstanceUrl );
			var uri = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, jobResponse.ContentUrl ).Uri;
			var request = System.Net.WebRequest.CreateHttp( uri );
			request.Headers.Add( "Authorization", "Bearer " + loginResponse.AccessToken );
#if DEBUG
			request.Headers.Add( "Accept-Encoding", "identity, gzip, deflate" );
#else
			request.Headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			request.ContentType = "text/csv";
			request.Method = System.Net.Http.HttpMethod.Put.Method;
			using ( var w = request.GetRequestStream() ) {
				var buffer = System.Text.Encoding.UTF8.GetBytes( data );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				var failure = System.Net.HttpStatusCode.Created != sc;
				if ( failure ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
				}
			}
		}

		protected System.Collections.Generic.IEnumerable<System.String> BuildFiles( System.Collections.Generic.IEnumerable<System.Data.DataTable> tables, System.Char columnDelimiter, System.String lineEnding, System.Char quoteChar, System.Int32 batchSize ) {
			return tables.Select(
				x => this.BuildFile( x, columnDelimiter, lineEnding, quoteChar )
			).SelectMany(
				x => BreakFile( x, batchSize )
			).SelectMany(
				x => BreakFile( MaxFileSize, x )
			);
		}
		protected System.String BuildFile( System.Data.DataTable dataTable, System.Char columnDelimiter, System.String lineEnding, System.Char quoteChar ) {
			if ( null == dataTable ) {
				throw new System.ArgumentNullException( "dataTable" );
			}
			var dbColumns = dataTable.Columns.OfType<System.Data.DataColumn>();
			if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			}

			var qcs = quoteChar.ToString();
			var columnNameList = dbColumns.Select(
				x => x.ColumnName.Replace( qcs, qcs + qcs )
			);
			var rs = lineEnding;
			var list = columnNameList.Select(
				x => qcs + x + qcs
			);
			using ( var writer = new System.IO.StringWriter() ) {
				writer.Write( System.String.Join( columnDelimiter.ToString(), list ) );
				writer.Write( lineEnding );
				foreach ( var row in dataTable.Rows.OfType<System.Data.DataRow>() ) {
					writer.Write( this.GetRow( dbColumns, row, columnDelimiter, lineEnding, quoteChar ) + lineEnding );
				}
				return writer.ToString();
			}
		}
		protected System.String GetRow( System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row, System.Char columnDelimiter, System.String lineEnding, System.Char quoteChar ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			}

			var qcs = quoteChar.ToString();
			var fss = columnDelimiter.ToString();
			var valueList = columns.Select(
				x => System.String.Format( "{0}", row[ x ] ).Replace( qcs, qcs + qcs )
			);
			var rs = lineEnding;
			var list = valueList.Select(
				x => ( x.Contains( qcs ) || x.Contains( fss ) || x.Contains( rs ) )
					? qcs + x + qcs
					: x
			);
			return System.String.Join( fss, list );
		}
		protected static System.Collections.Generic.IEnumerable<System.String> BreakFile( System.String file, System.Int32 recordCount ) {
			if ( recordCount <= 0 ) {
				throw new System.ArgumentOutOfRangeException( "recordCount", "recordCount parameter must be positive." );
			} else if ( System.String.IsNullOrEmpty( file ) ) {
				throw new System.ArgumentNullException( "file" );
			}
			var le = LineEndingOption.CRLF.Value;
			using ( var reader = new System.IO.StringReader( file ) ) {
				var output = new System.Text.StringBuilder();
				var headerLine = reader.ReadLine( le, '"' ) + le;
				output = output.Append( headerLine );
				System.Int32 i = 0;
				while ( EOF != reader.Peek() ) {
					output = output.Append( reader.ReadLine( le, '"' ) + le );
					if ( recordCount <= ++i ) {
						i = 0;
						yield return output.ToString();
						if ( EOF == reader.Peek() ) {
							yield break;
						} else {
							output = new System.Text.StringBuilder();
							output = output.Append( headerLine );
						}
					}
				}
				yield return output.ToString();
			}
		}
		protected static System.Collections.Generic.IEnumerable<System.String> BreakFile( System.Int32 maxSize, System.String file ) {
			if ( System.String.IsNullOrEmpty( file ) ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( maxSize <= 1048576 ) {
				throw new System.ArgumentOutOfRangeException( "maxSize", "maxSize parameter must be at least 1048576." );
			}

			var maxCharSize = file.Length << 2;
			if ( maxCharSize <= maxSize ) {
				yield return file;
				yield break;
			}
			var actualSize = System.Text.Encoding.UTF8.GetByteCount( file );
			if ( actualSize <= maxSize ) {
				yield return file;
				yield break;
			}
			System.Func<System.String, System.Int32> getSize = x => System.Text.Encoding.UTF8.GetByteCount( x );
			var le = LineEndingOption.CRLF.Value;
			using ( var reader = new System.IO.StringReader( file ) ) {
				var output = new System.Text.StringBuilder();
				var headerLine = reader.ReadLine( le, '"' ) + le;
				output = output.Append( headerLine );
				System.Int32 runningSize = output.Length;
				System.Int32 probeSize;
				System.String line;
				while ( EOF != reader.Peek() ) {
					line = reader.ReadLine( le, '"' ) + le;
					probeSize = getSize( line );
					if ( ( runningSize + probeSize ) < maxSize ) {
						output = output.Append( line );
						runningSize += probeSize;
					} else {
						yield return output.ToString();
						output = new System.Text.StringBuilder();
						output = output.Append( headerLine );
						output = output.Append( line );
						runningSize = output.Length;
					}
				}
				yield return output.ToString();
			}
		}
		#endregion methods

	}

}