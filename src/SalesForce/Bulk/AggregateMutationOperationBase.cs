// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public abstract class AggregateMutationOperationBase : AggregateOperationBase {

		#region  fields
		public const System.Data.MissingMappingAction DefaultMissingMappingAction = System.Data.MissingMappingAction.Passthrough;
		public const System.Data.MissingSchemaAction DefaultMissingSchemaAction = System.Data.MissingSchemaAction.Add;
		public const System.Int32 EOF = -1;
		public const System.Int32 MinimumMaxFileSize = 1048576;
		public const System.Int32 MaxFileSize = 103809024;

		private const System.String FailedResults = "failedResults";
		private const System.String SuccessfulResults = "successfulResults";
		#endregion  fields


		#region  .ctor
		protected AggregateMutationOperationBase() : base() {
			this.MissingMappingAction = DefaultMissingMappingAction;
			this.MissingSchemaAction = DefaultMissingSchemaAction;
			this.ApiVersion = DefaultApiVersion;
		}
		protected AggregateMutationOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			this.MissingMappingAction = DefaultMissingMappingAction;
			this.MissingSchemaAction = DefaultMissingSchemaAction;
			this.ApiVersion = DefaultApiVersion;
		}
		protected AggregateMutationOperationBase( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = DefaultMissingMappingAction;
			this.MissingSchemaAction = DefaultMissingSchemaAction;
			this.ApiVersion = DefaultApiVersion;
		}
		protected AggregateMutationOperationBase( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
			this.MissingMappingAction = DefaultMissingMappingAction;
			this.MissingSchemaAction = DefaultMissingSchemaAction;
			this.ApiVersion = DefaultApiVersion;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"missingMappingAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultMissingMappingAction )]
		public sealed override System.Data.MissingMappingAction MissingMappingAction {
			get {
				return base.MissingMappingAction;
			}
			set {
				base.MissingMappingAction = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"missingSchemaAction",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultMissingSchemaAction )]
		public sealed override System.Data.MissingSchemaAction MissingSchemaAction {
			get {
				return base.MissingSchemaAction;
			}
			set {
				base.MissingSchemaAction = value;
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

		[System.Xml.Serialization.XmlElement(
			"success",
			Namespace = "http://Icod.Wod"
		)]
		public DbDestination Success {
			get;
			set;
		}
		[System.Xml.Serialization.XmlElement(
			"error",
			Namespace = "http://Icod.Wod"
		)]
		public DbDestination Error {
			get;
			set;
		}
		#endregion properties


		#region methods
		public override void PerformWork( Pair<LoginResponse, IStep> jobProcess ) {
			var step = jobProcess.Second;
			if ( null == step ) {
				throw new System.ArgumentException();
			}
			var workOrder = step.WorkOrder;
			var loginResponse = jobProcess.First;

			System.String lineEnding = LineEndingOption.CRLF.Value;
			foreach ( var file in this.BuildFiles(
				this.ReadTables( workOrder ),
				ColumnDelimiterOption.Comma.Value, lineEnding, '"',
				this.BatchSize
			) ) {
				var jobResponse = this.CreateJob( loginResponse );
				var id = jobResponse.Id;

				var wait = ( this.Wait ?? new Wait() );
				var max = wait.Maximum;
				var sleepTime = wait.Initial;
				if (
					!StateOption.Open.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
				) {
					if ( 0 < sleepTime ) {
						System.Threading.Thread.Sleep( sleepTime );
					}
					jobResponse = this.QueryJob( loginResponse, id );
					sleepTime = wait.Minimum;
					while (
						!StateOption.Open.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& ( sleepTime < max )
					) {
						System.Threading.Thread.Sleep( sleepTime );
						sleepTime = System.Math.Min( max, sleepTime + wait.Increment );
						jobResponse = this.QueryJob( loginResponse, id );
					}
					while (
						!StateOption.Open.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					) {
						System.Threading.Thread.Sleep( max );
						jobResponse = this.QueryJob( loginResponse, id );
					}
				}

				this.UploadData( loginResponse, jobResponse, file );

				this.CloseJob( loginResponse, id );

				jobResponse = this.QueryJob( loginResponse, id );
				sleepTime = wait.Initial;
				if (
					!StateOption.JobComplete.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
				) {
					if ( 0 < sleepTime ) {
						System.Threading.Thread.Sleep( sleepTime );
					}
					jobResponse = this.QueryJob( loginResponse, id );
					sleepTime = wait.Minimum;
					while (
						!StateOption.JobComplete.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& ( sleepTime < max )
					) {
						System.Threading.Thread.Sleep( sleepTime );
						sleepTime = System.Math.Min( max, sleepTime + wait.Increment );
						jobResponse = this.QueryJob( loginResponse, id );
					}
					while (
						!StateOption.JobComplete.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Aborted.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
						&& !StateOption.Failed.Value.Equals( jobResponse.state, System.StringComparison.OrdinalIgnoreCase )
					) {
						System.Threading.Thread.Sleep( max );
						jobResponse = this.QueryJob( loginResponse, id );
					}
				}

				if ( null != this.Error ) {
					var error = this.GetResults( workOrder, loginResponse, jobResponse, FailedResults );
					if ( !System.String.IsNullOrEmpty( error.Body ) ) {
						this.Error.WriteRecords( workOrder, error );
					}
				}
				if ( null != this.Success ) {
					var success = this.GetResults( workOrder, loginResponse, jobResponse, SuccessfulResults );
					if ( !System.String.IsNullOrEmpty( success.Body ) ) {
						this.Success.WriteRecords( workOrder, success );
					}
				}

				this.DeleteJob( loginResponse, id );
			}
		}

		private SelectResult GetResults( WorkOrder workOrder, LoginResponse loginResponse, JobResponse jobResponse, System.String results ) {
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
						AdditionalColumns = new System.Data.DataColumn[ 7 ] {
							new System.Data.DataColumn( "%wod:sf_operation%", typeof( System.String ) ) {
								DefaultValue = jobResponse.operation
							},
							new System.Data.DataColumn( "%wod:sf_object%", typeof( System.String ) ) {
								DefaultValue = this.Object
							},
							new System.Data.DataColumn( "%wod:sf_jobType%", typeof( System.String ) ) {
								DefaultValue = jobResponse.JobType.ToString()
							},
							new System.Data.DataColumn( "%wod:sf_resultType%", typeof( System.String ) ) {
								DefaultValue = results
							},
							new System.Data.DataColumn( "%wod:sf_tag%", typeof( System.String ) ) {
								DefaultValue = this.Tag
							},
							new System.Data.DataColumn( "%wod:EmailTo%", typeof( System.String ) ) {
								DefaultValue = workOrder.EmailTo
							},
							new System.Data.DataColumn( "%wod:JobName%", typeof( System.String ) ) {
								DefaultValue = workOrder.JobName
							},
						},
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
			var mapping = ( this.ColumnMapping ?? new Data.ColumnMap[ 0 ] );
			foreach ( var skip in mapping.Where(
				x => x.Skip
			).Join(
				dataTable.Columns.OfType<System.Data.DataColumn>(),
				outer => outer.FromName,
				inner => inner.ColumnName,
				( outer, inner ) => outer.FromName,
				System.StringComparer.OrdinalIgnoreCase
			) ) {
				dataTable.Columns.Remove( skip );
			}

			var qcs = quoteChar.ToString();
			var columnNameList = dbColumns.Select(
				x => x.ColumnName.Replace( qcs, qcs + qcs )
			);
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
			var list = valueList.Select(
				x => qcs + x + qcs
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
			} else if ( maxSize <= MinimumMaxFileSize ) {
				throw new System.ArgumentOutOfRangeException( "maxSize", System.String.Format( "maxSize parameter must be equal to or greater than the MinimumMaxFileSize, {0}.", MinimumMaxFileSize ) );
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
