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
			myEolWriter( writer );
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
			myEolWriter( writer );
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