using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"delimitedWriter",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class DelimitedFileWriter : FileBase {

		#region fields
		private System.Char myFieldSeparator;
		private System.String myFieldSeparatorString;
		private System.Char myQuoteChar;
		private System.Nullable<System.Char> myEscapeChar;
		#endregion fields


		#region .ctor
		public DelimitedFileWriter() : base() {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myEscapeChar = null;
		}
		public DelimitedFileWriter( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myEscapeChar = null;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"fieldSeperator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 9 )]
		public System.Int32 FieldSeparatorNumber {
			get {
				return System.Convert.ToInt32( myFieldSeparator );
			}
			set {
				myFieldSeparator = System.Convert.ToChar( value );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"quoteChar",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 34 )]
		public System.Int32 QuoteCharNumber {
			get {
				return System.Convert.ToInt32( myQuoteChar );
			}
			set {
				myQuoteChar = System.Convert.ToChar( value );
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"escapeChar",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( -1 )]
		public System.Int32 EscapeCharNumber {
			get {
				var ec = EscapeChar;
				return ec.HasValue
					? System.Convert.ToInt32( myEscapeChar )
					: -1
				;
			}
			set {
				if ( value <= -2 ) {
					throw new System.InvalidOperationException();
				} else if ( -1 == value ) {
					myEscapeChar = null;
				} else {
					myEscapeChar = System.Convert.ToChar( value );
				}
			}
		}

		[System.ComponentModel.DefaultValue( '\t' )]
		[System.Xml.Serialization.XmlIgnore]
		public System.Char FieldSeparator {
			get {
				return myFieldSeparator;
			}
			set {
				myFieldSeparator = value;
				myFieldSeparatorString = value.ToString();
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.DefaultValue( "\t" )]
		public System.String FieldSeparatorString {
			get {
				return myFieldSeparatorString;
			}
		}

		[System.ComponentModel.DefaultValue( '\"' )]
		[System.Xml.Serialization.XmlIgnore]
		public System.Char QuoteChar {
			get {
				return myQuoteChar;
			}
			set {
				myQuoteChar = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Nullable<System.Char> EscapeChar {
			get {
				return myEscapeChar;
			}
			set {
				myEscapeChar = value;
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
				using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength) ) {
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
			writer.WriteLine( System.String.Join( this.FieldSeparatorString, columns.Select(
				x => x.ColumnName
			).ToArray() ) );
		}
		private void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			writer.WriteLine( System.String.Join( this.FieldSeparatorString, columns.Select(
				x => row[ x ]
			).Select(
				x => ( x ?? System.String.Empty ).ToString()
			) ) );
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