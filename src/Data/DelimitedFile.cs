using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"delimitedFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class DelimitedFile : FileBase {

		#region fields
		private System.Char myFieldSeparator;
		private System.String myFieldSeparatorString;
		private System.Char myQuoteChar;
		private System.String myQuoteCharString;
		private System.Nullable<System.Char> myEscapeChar;
		#endregion fields


		#region .ctor
		public DelimitedFile() : base() {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myQuoteCharString = myQuoteChar.ToString();
			myEscapeChar = null;
		}
		public DelimitedFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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
				var ec = this.EscapeChar;
				return ec.HasValue
					? System.Convert.ToInt32( ec.Value )
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
		protected sealed override System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			var output = new System.Data.DataTable();
			if ( !this.HasHeader ) {
				if ( !( this.Columns ?? new TextFileColumn[ 0 ] ).Any() ) {
					throw new System.InvalidOperationException();
				}
				System.Data.DataColumn col;
				System.Int32 l;
				foreach ( var c in this.Columns ) {
					col = new System.Data.DataColumn( c.Name );
					l = c.Length;
					if ( -1 != l ) {
						col.MaxLength = l;
					}
					output.Columns.Add( col );
				}
			} else {
				foreach ( var c in this.ReadHeaderLine( file ) ) {
					output.Columns.Add( new System.Data.DataColumn( c ) );
				}
			}
			var fileNameColumn = new System.Data.DataColumn( "%wod:FilePathName%", typeof( System.String ) );
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

			var row = this.ReadRecord( file );
			var rowList = row.ToArray();
			return table.Rows.Add( rowList );
		}
		private System.Collections.Generic.IEnumerable<System.String> ReadHeaderLine( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
			var output = this.ReadRecord( file );
			return output;
		}

		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				yield break;
			}
			var line = file.ReadLine();
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				var reading = true;
				System.Char c;
				System.String column;
				var ec = this.EscapeChar;
				var qc = this.QuoteChar;
				do {
					i = reader.Peek();
					if ( -1 == i ) {
						reading = false;
						break;
					}
					c = System.Convert.ToChar( i );
					if ( ec.HasValue && ( ec.Value == c ) ) {
						reader.Read();
						column = this.ReadNormalColumn( reader, System.Convert.ToChar( reader.Read() ) );
						yield return column;
					} else if ( qc == c ) {
						reader.Read();
						column = this.ReadQuotedColumn( reader, System.Convert.ToChar( reader.Read() ) );
						yield return column;
					} else {
						column = this.ReadNormalColumn( reader, null );
						yield return column;
					}
				} while ( reading );
			}
		}
		private System.String ReadNormalColumn( System.IO.StringReader reader, System.Nullable<System.Char> first ) {
			return ReadColumn( reader, first, this.FieldSeparator, false );
		}
		private System.String ReadQuotedColumn( System.IO.StringReader reader, System.Nullable<System.Char> first ) {
			return ReadColumn( reader, first, this.QuoteChar, true );
		}

		private System.String ReadColumn( System.IO.StringReader reader, System.Nullable<System.Char> first, System.Char @break, System.Boolean readNextOnBreak ) {
			System.Text.StringBuilder sb = new System.Text.StringBuilder( 128 );
			if ( first.HasValue ) {
				sb.Append( first.Value );
			}
			System.Char c;
			System.Int32 i;
			System.Boolean reading = true;
			var ec = this.EscapeChar;
			do {
				i = reader.Read();
				if ( -1 == i ) {
					reading = false;
					break;
				}
				c = System.Convert.ToChar( i );
				if ( ec.HasValue && ( ec.Value == c ) ) {
					c = System.Convert.ToChar( reader.Read() );
				} else if ( @break == c ) {
					if ( readNextOnBreak ) {
						reader.Read();
					}
					reading = false;
				}
				if ( reading ) {
					sb.Append( c );
				}
			} while ( reading );
			return sb.ToString().TrimToNull();
		}


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
		#endregion methods

	}

}