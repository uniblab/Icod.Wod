using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fixedWidthFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class FixedWidthFile : FileBase {

		#region fields
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;

		private System.String myRecordSeparator;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		#endregion fields


		#region .ctor
		static FixedWidthFile() {
			theEmptyEolWriter = x => {
			};
		}

		public FixedWidthFile() : base() {
			myRecordSeparator = "\r\n";
			myEolWriter = theEmptyEolWriter;
		}
		public FixedWidthFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myRecordSeparator = "\r\n";
			myEolWriter = theEmptyEolWriter;
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
		[System.Xml.Serialization.XmlIgnore]
		protected System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? theEmptyEolWriter;
			}
		}
		#endregion properties


		#region methods
		protected sealed override System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			var output = new System.Data.DataTable();
			if ( !( this.Columns ?? new TextFileColumn[ 0 ] ).Any() ) {
				throw new System.InvalidOperationException();
			}
			System.Data.DataColumn col;
			System.Int32 l;
			foreach ( var c in this.Columns ) {
				col = new System.Data.DataColumn( c.Name );
				l = c.Length;
				if ( l <= 0 ) {
					throw new System.InvalidOperationException();
				}
				col.MaxLength = l;
				output.Columns.Add( col );
			}
			var fileNameColumn = new System.Data.DataColumn( "%wod:FileName%", typeof( System.String ) );
			fileNameColumn.AllowDBNull = false;
			fileNameColumn.ReadOnly = true;
			fileNameColumn.DefaultValue = fileName;
			output.Columns.Add( fileNameColumn );
			output.TableName = fileName;

			return output;
		}
		protected sealed override System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			}
			return table.Rows.Add( this.ReadRecord( file ).ToArray() );
		}
		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				yield break;
			}

			var cols = this.Columns.OfType<System.Data.DataColumn>().Where(
				x => x.ColumnName.StartsWith( "%wod:", System.StringComparison.OrdinalIgnoreCase ) && x.ColumnName.EndsWith( "%", System.StringComparison.OrdinalIgnoreCase )
			);
			var bufferLen = cols.Select(
				x => x.MaxLength
			).Sum() + ( this.RecordSeparator ?? System.String.Empty ).Length;
			var buffer = new System.Char[ bufferLen ];
			System.Int32 r = file.ReadBlock( buffer, 0, bufferLen );
			if ( 0 == r ) {
				yield break;
			}
			var record = new System.String( buffer, 0, r );
			System.Int32 l;
			System.Int32 i = 0;
			foreach ( var c in cols ) {
				l = c.MaxLength;
				yield return record.Substring( i, l ).TrimToNull();
				i += l;
			}
		}

		protected sealed override void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns ) {
			if ( ( null == fileColumns ) || !fileColumns.Any() ) {
				throw new System.ArgumentNullException( "fileColumns" );
			} else if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			var line = dbColumns.Select(
				x => x.ColumnName
			).Where(
				x => fileColumns.Select(
					y => y.Name
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => x.PadRight( fileColumns.First(
					y => y.Name.Equals( x )
				).Length, ' ' ).Substring( 0, this.Columns.First(
					y => y.Name.Equals( x )
				).Length )
			);
			writer.Write( line );
			this.EolWriter( writer );
		}
		protected sealed override void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns, System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			} else if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			TextFileColumn tfc = null;
			foreach ( var dbCol in dbColumns ) {
				tfc = formatMap[ dbCol ];
				writer.Write( System.String.Format( tfc.FormatString ?? "{0}", row[ dbCol ] ?? System.String.Empty ).PadRight( tfc.Length ).Substring( 0, tfc.Length ) );
			}
			this.EolWriter( writer );
		}
		protected sealed override void WriteFile( System.IO.Stream stream ) {
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
		#endregion  methods

	}

}