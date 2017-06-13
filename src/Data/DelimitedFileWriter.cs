using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"delimitedWriter",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class DelimitedFileWriter : FileBase {

		#region fields
		private System.Char myFieldSeparator;
		private System.String myFieldSeparatorString;
		private System.Char myQuoteChar;
		private System.String myQuoteCharString;
		private System.Nullable<System.Char> myEscapeChar;
		#endregion fields


		#region .ctor
		public DelimitedFileWriter() : base() {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myQuoteCharString = myQuoteChar.ToString();
			myEscapeChar = null;
		}
		public DelimitedFileWriter( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myQuoteCharString = myQuoteChar.ToString();
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
				myFieldSeparatorString = myFieldSeparator.ToString();
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
				myQuoteCharString = myQuoteChar.ToString();
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
				myQuoteCharString = myQuoteChar.ToString();
			}
		}
		[System.ComponentModel.DefaultValue( '\"' )]
		[System.Xml.Serialization.XmlIgnore]
		public System.String QuoteCharString {
			get {
				return myQuoteCharString;
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
		protected sealed override void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns ) {
			if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}
			writer.WriteLine( System.String.Join( this.FieldSeparatorString, dbColumns.Select(
				x => x.ColumnName
			).ToArray() ) );
		}
		protected sealed override void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns, System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			} else if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}


			var s = this.FieldSeparatorString;
			System.String q = this.QuoteCharString;
			System.Collections.Generic.IList<System.String> line = new System.Collections.Generic.List<System.String>();
			System.String c = null;
			System.Text.StringBuilder cb = null;
			foreach ( var col in dbColumns ) {
				c = System.String.Format( formatMap[ col ].FormatString ?? "{0}", row[ col ] ?? System.String.Empty );
				if ( c.Contains( s ) ) {
					cb = new System.Text.StringBuilder();
					if ( !c.StartsWith( q ) ) {
						cb.Append( q );
					}
					cb.Append( c );
					if ( !c.EndsWith( q ) ) {
						cb.Append( q );
					}
					line.Add( cb.ToString() );
				} else {
					line.Add( c );
				}
			}
			writer.WriteLine( System.String.Join( s, line ) );
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