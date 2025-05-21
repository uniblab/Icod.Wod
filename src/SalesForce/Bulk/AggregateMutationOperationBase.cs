// Copyright (C) 2025  Timothy J. Bruce
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
		private const System.String UnprocessedResults = "unprocessedrecords";
		#endregion  fields


		#region  .ctor
		protected AggregateMutationOperationBase() : base() {
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
		[System.Xml.Serialization.XmlElement(
			"unprocessed",
			Namespace = "http://Icod.Wod"
		)]
		public DbDestination Unprocessed {
			get;
			set;
		}
		#endregion properties


		#region methods
		protected sealed override System.String GetServicePath() {
			return System.String.Format( "/services/data/v{0:F1}/jobs/ingest", this.ApiVersion );
		}
		public override void PerformWork( Pair<LoginResponse, IStep> jobProcess ) {
			var step = jobProcess.Second ?? throw new System.ArgumentException( nameof( jobProcess ) );
			var workOrder = step.WorkOrder;
			var loginResponse = jobProcess.First;

#if DEBUG
			System.String lineEnding = LineEndingOption.CRLF.Value;
#else
			System.String lineEnding = LineEndingOption.LF.Value;
#endif
			foreach ( var file in this.BuildFiles(
				this.ReadTables( workOrder ),
				ColumnDelimiterOption.Comma.Value, lineEnding, '"',
				this.BatchSize
			) ) {
				var jobResponse = this.CreateJob( loginResponse );
				var id = jobResponse.Id;

				jobResponse = this.WaitUntilStateOption( 
					jobResponse, loginResponse, id, 
					new StateOption[ 3 ] { StateOption.Aborted, StateOption.Failed, StateOption.Open }
				);
				if ( StateOption.Open.Equals( jobResponse.State ) ) {
					this.UploadData( loginResponse, jobResponse, file );
					this.SendUploadComplete( loginResponse, id );
				}

				jobResponse = this.WaitUntilStateOption(
					this.QueryJob( loginResponse, id ), loginResponse, id,
					new StateOption[ 3 ] { StateOption.Aborted, StateOption.Failed, StateOption.JobComplete }
				);

				if ( this.Error is object ) {
					var error = this.GetResults( workOrder, loginResponse, jobResponse, FailedResults );
					if ( !System.String.IsNullOrEmpty( error.Body ) ) {
						this.Error.WriteRecords( workOrder, error );
					}
				}
				if ( this.Success is object ) {
					var success = this.GetResults( workOrder, loginResponse, jobResponse, SuccessfulResults );
					if ( !System.String.IsNullOrEmpty( success.Body ) ) {
						this.Success.WriteRecords( workOrder, success );
					}
				}
				if ( this.Unprocessed is object ) {
					var unprocessed = this.GetResults( workOrder, loginResponse, jobResponse, UnprocessedResults );
					if ( !System.String.IsNullOrEmpty( unprocessed.Body ) ) {
						this.Unprocessed.WriteRecords( workOrder, unprocessed );
					}
				}

				this.DeleteJob( loginResponse, id );
			}
		}

		private SelectResult GetResults( WorkOrder workOrder, LoginResponse loginResponse, JobResponse jobResponse, System.String results ) {
			var id = jobResponse.Id;
			var request = this.BuildSalesForceRequest( loginResponse, id, "text/csv", "GET", results );
			using ( var response = request.GetResponse() ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var source = response.GetResponseStream() ) {
						source.CopyTo( buffer );
					}
					return new SelectResult {
						Body = buffer.ToArray().GetWebString( System.Text.Encoding.UTF8, response.Headers[ "Content-Encoding" ].TrimToNull() ?? "identity" ),
						ColumnDelimiter = ColumnDelimiterOption.FromName( jobResponse.ColumnDelimiter ).Value,
						LineEnding = LineEndingOption.FromName( jobResponse.LineEnding ).Value,
						AdditionalColumns = new System.Data.DataColumn[ 7 ] {
							new System.Data.DataColumn( "%wod:sf_operation%", typeof( System.String ) ) {
								DefaultValue = jobResponse.Operation
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
			var request = this.BuildSalesForceRequest( loginResponse, "application/json; charset=utf-8", "POST" );
			using ( var w = request.GetRequestStream() ) {
				var jr = new {
					operation = operation,
					columnDelimiter = "COMMA",
					contentType = "CSV",
#if DEBUG
					lineEnding = "CRLF",
#else
					lineEnding = "LF",
#endif
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
		protected void SendUploadComplete( LoginResponse loginResponse, System.String id ) {
			var request = this.BuildSalesForceRequest( loginResponse, id, "application/json; charset=utf-8", "PATCH" );
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
			var request = this.BuildSalesForceRequest( loginResponse, null, "text/csv", "PUT", null, jobResponse.ContentUrl, null );
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
			dataTable = dataTable ?? throw new System.ArgumentNullException( nameof( dataTable ) );
			var dbColumns = dataTable.Columns.OfType<System.Data.DataColumn>();
			if ( ( dbColumns is null ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( dbColumns ) );
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
					writer.Write( this.GetRow( dbColumns, row, columnDelimiter, quoteChar ) + lineEnding );
				}
				return writer.ToString();
			}
		}
		protected System.String GetRow( System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row, System.Char columnDelimiter, System.Char quoteChar ) {
			row = row ?? throw new System.ArgumentNullException( nameof( row ) );
			if ( ( columns is null ) || !columns.Any() ) {
				throw new System.ArgumentNullException( nameof( columns ) );
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
				throw new System.ArgumentNullException( nameof( file ) );
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
				throw new System.ArgumentNullException( nameof( file ) );
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
			System.Int32 getSize( System.String x ) => System.Text.Encoding.UTF8.GetByteCount( x );
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
