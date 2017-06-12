using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fixedWidthWriter",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FixedWidthFileWriter : FileBase {

		#region fields
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;

		private System.String myRecordSeparator;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		#endregion fields


		#region .ctor
		static FixedWidthFileWriter() {
			theEmptyEolWriter = x => {};
		}

		public FixedWidthFileWriter() : base() {
			myRecordSeparator = "\r\n";
		}
		public FixedWidthFileWriter( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myRecordSeparator = "\r\n";
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "\r\n" )]
		public System.String RecordSeparator {
			get {
				return myRecordSeparator;
			}
			set {
				myRecordSeparator = value;
				myEolWriter = System.String.IsNullOrEmpty( value )
					? theEmptyEolWriter
					: x => x.Write( myRecordSeparator )
				;
			}
		}
		#endregion properties


		#region methods
		public sealed override void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength ) ) {
					var table = source.ReadTables( order ).FirstOrDefault();
					var columns = table.Columns.OfType<System.Data.DataColumn>();
					if ( this.HasHeader ) {
						this.WriteHeader( writer, columns );
					}
					foreach ( var row in table.Rows.OfType<System.Data.DataRow>() ) {
						this.WriteRow( writer, columns, row );
					}
					writer.Flush();
					this.WriteFile( buffer );
				}
			}
		}
		private void WriteHeader( System.IO.StreamWriter writer, System.Data.DataTable table ) {
			if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			this.WriteHeader( writer, table.Columns.OfType<System.Data.DataColumn>() );
		}
		private void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns ) {
			if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			var line = columns.Select(
				x => x.ColumnName
			).Where(
				x => this.Columns.Select(
					y => y.Name
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => x.PadRight( this.Columns.First( 
					y => y.Name.Equals( x )
				).Length, ' ' ).Substring( 0, this.Columns.First(
					y => y.Name.Equals( x )
				).Length )
			);
			writer.Write( line );
			myEolWriter( writer );
		}
		private void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			var line = columns.Where(
				x => this.Columns.Select(
					y => y.Name
				).Contains( x.ColumnName, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => ( row[ x ] ?? System.String.Empty ).ToString().PadRight( this.Columns.First(
					y => y.Name.Equals( x.ColumnName )
				).Length, ' ' ).Substring( 0, this.Columns.First(
					y => y.Name.Equals( x.ColumnName )
				).Length )
			);
			writer.Write( line );
			myEolWriter( writer );
		}
		private void WriteFile( System.IO.Stream stream ) {
			if ( null == stream ) {
				throw new System.ArgumentNullException( "stream" );
			}
			var handler = this.GetFileHandler( this.WorkOrder );
			var dfpn = handler.PathCombine( this.ExpandedPath, this.ExpandedName );
			stream.Seek( 0, System.IO.SeekOrigin.Begin );
			if ( this.Append ) {
				handler.Append( stream, dfpn );
			} else {
				handler.Overwrite( stream, dfpn );
			}
		}

		protected sealed override System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file ) {
			throw new System.NotImplementedException();
		}
		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
			throw new System.NotImplementedException();
		}
		protected sealed override System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file ) {
			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}