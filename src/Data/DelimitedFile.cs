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
		protected sealed override System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
			if ( !this.HasHeader ) {
				if ( !( this.Columns ?? new TextFileColumn[ 0 ] ).Any() ) {
					throw new System.InvalidOperationException();
				}
				return this.Columns.Select(
					x => new System.Data.DataColumn( x.Name, typeof( System.String ) )
				);
			} else {
				return this.ReadHeaderLine( file ).Select(
					x => new System.Data.DataColumn( x, typeof( System.String ) )
				);
			}
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
			} else if ( System.String.IsNullOrEmpty( this.RecordSeparator ) ) {
				throw new System.InvalidOperationException();
			}

			var line = file.ReadLine( this.RecordSeparator );
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
					if ( ec.HasValue && ( ec.Value.Equals( c ) ) ) {
						reader.Read();
						column = this.ReadNormalColumn( reader, System.Convert.ToChar( reader.Read() ) );
						yield return column;
					} else if ( qc.Equals( c ) ) {
						reader.Read();
						column = this.ReadQuotedColumn( reader, null );
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
			System.Char ch;
			System.Int32 i;
			System.Boolean reading = true;
			var ec = this.EscapeChar;
			do {
				i = reader.Read();
				if ( -1 == i ) {
					reading = false;
					break;
				}
				ch = System.Convert.ToChar( i );
				if ( ec.HasValue && ( ec.Value == ch ) ) {
					ch = System.Convert.ToChar( reader.Read() );
				} else if ( @break == ch ) {
					if ( readNextOnBreak ) {
						reader.Read();
					}
					reading = false;
				}
				if ( reading ) {
					sb.Append( ch );
				}
			} while ( reading );
			System.Func<System.Text.StringBuilder, System.String> w = a => a.ToString();
			var q = ( this.TrimValues )
				? a => w( a ).TrimToNull()
				: w
			;
			return ( this.ConvertEmptyStringToNull ) ? q( sb ) ?? System.String.Empty : q( sb );
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
		protected sealed override System.String GetRow( System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			return System.String.Join( this.FieldSeparatorString, columns.Select(
				x => this.GetColumn( 
					formatMap.ContainsKey( x )
						? formatMap[ x ] ?? new TextFileColumn( x.ColumnName )
						: new TextFileColumn( x.ColumnName )
					,
					x,
					row
				)
			) );
		}
		#endregion methods

	}

}