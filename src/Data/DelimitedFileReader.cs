using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"delimitedReader",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class DelimitedFileReader : FileBase {

		#region fields
		private System.Char myFieldSeparator;
		private System.Char myQuoteChar;
		private System.Nullable<System.Char> myEscapeChar;
		#endregion fields


		#region .ctor
		public DelimitedFileReader() : base() {
			myFieldSeparator = '\t';
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
			throw new System.NotImplementedException();
		}

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
				System.Boolean reading = true;
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
		#endregion methods

	}

}