using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"delimitedFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class DelimitedFile : TextFileBase {

		#region fields
		private static readonly System.Func<System.Text.StringBuilder, System.String> theValueToString;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theTrimToNullReader;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theTrimReader;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theDefaultColumnReader;

		private System.Char myFieldSeparator;
		private System.String myFieldSeparatorString;
		private System.Char myQuoteChar;
		private System.String myQuoteCharString;
		private System.Nullable<System.Char> myEscapeChar;
		private System.Int32 myEscapeCharNumber;
		private System.Func<System.Text.StringBuilder, System.String> myColumnReader;
		private System.Boolean myForceQuote;
		#endregion fields


		#region .ctor
		static DelimitedFile() {
			theValueToString = a => ( null == a ) 
				? null 
				: ( System.DBNull.Value.Equals( a ) ) 
					? null 
					: a.ToString()
			;
			theTrimReader = a => ( theValueToString( a ) ?? System.String.Empty ).Trim();
			theTrimToNullReader = a => theValueToString( a ).TrimToNull();

			theDefaultColumnReader = x => theTrimToNullReader( x );
		}

		public DelimitedFile() : base() {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myQuoteCharString = myQuoteChar.ToString();
			myEscapeChar = null;
			myEscapeCharNumber = -1;
			myColumnReader = theDefaultColumnReader;
			myForceQuote = true;
		}
		public DelimitedFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myFieldSeparator = '\t';
			myFieldSeparatorString = myFieldSeparator.ToString();
			myQuoteChar = '\"';
			myQuoteCharString = myQuoteChar.ToString();
			myEscapeChar = null;
			myEscapeCharNumber = -1;
			myColumnReader = theDefaultColumnReader;
			myForceQuote = true;
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
				return myEscapeCharNumber;
			}
			set {
				if ( value <= -2 ) {
					throw new System.InvalidOperationException();
				} else if ( -1 == value ) {
					myEscapeChar = null;
				} else {
					myEscapeChar = System.Convert.ToChar( value );
				}
				myEscapeCharNumber = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.DefaultValue( null )]
		public System.Nullable<System.Char> EscapeChar {
			get {
				return myEscapeChar;
			}
			set {
				if ( !value.HasValue ) {
					myEscapeCharNumber = -1;
				}
				myEscapeChar = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"convertEmptyStringToNull",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public sealed override System.Boolean ConvertEmptyStringToNull {
			get {
				return base.ConvertEmptyStringToNull;
			}
			set {
				base.ConvertEmptyStringToNull = value;
				this.ColumnReader = ( this.ConvertEmptyStringToNull )
					? theTrimToNullReader
					: ( this.TrimValues )
						? theTrimReader
						: theValueToString
				;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"trimValues",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public sealed override System.Boolean TrimValues {
			get {
				return base.TrimValues;
			}
			set {
				base.TrimValues = value;
				this.ColumnReader = ( this.ConvertEmptyStringToNull )
					? theTrimToNullReader
					: ( this.TrimValues )
						? theTrimReader
						: theValueToString
				;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		private System.Func<System.Text.StringBuilder, System.String> ColumnReader {
			get {
				return myColumnReader ?? theDefaultColumnReader;
			}
			set {
				myColumnReader = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"forceQuote",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean ForceQuote {
			get {
				return myForceQuote;
			}
			set {
				myForceQuote = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultRecordSeparator )]
		public sealed override System.String RecordSeparator {
			get {
				return base.RecordSeparator;
			}
			set {
				base.RecordSeparator = value;
				this.EolWriter = System.String.IsNullOrEmpty( value )
					? EmptyEolWriter
					: x => x.Write( value );
				;
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
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif
			var output = this.ReadRecord( file );
			return output;
		}

		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif
			if ( file.EndOfStream ) {
				yield break;
			} else if ( System.String.IsNullOrEmpty( this.RecordSeparator ) ) {
				throw new System.InvalidOperationException();
			}

			var line = file.ReadLine( this.RecordSeparator, this.QuoteChar );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String column;
				var reading = true;
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
						column = this.ReadColumn( reader, System.Convert.ToChar( reader.Read() ), this.FieldSeparator, false );
						yield return column;
					} else if ( qc.Equals( c ) ) {
						reader.Read();
						column = this.ReadColumn( reader, null, this.QuoteChar, true );
						yield return column;
					} else {
						column = this.ReadColumn( reader, null, this.FieldSeparator, false );
						yield return column;
					}
				} while ( reading );
			}
		}
		private System.String ReadColumn( System.IO.StringReader reader, System.Nullable<System.Char> first, System.Char @break, System.Boolean readNextOnBreak ) {
#if DEBUG
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif
			var sb = new System.Text.StringBuilder( 128 );
			if ( first.HasValue ) {
				sb.Append( first.Value );
			}
			System.Nullable<System.Char> ch;
			var reading = true;
			var ec = this.EscapeChar;
			do {
				ch = this.ReadChar( reader, ec, @break, readNextOnBreak );
				if ( ch.HasValue ) {
					sb = sb.Append( ch.Value );
				} else {
					reading = false;
					break;
				}
			} while ( reading );
			return this.ColumnReader( sb );
		}
		private System.Nullable<System.Char> ReadChar( System.IO.StringReader reader, System.Nullable<System.Char> escape, System.Char @break, System.Boolean readNextOnBreak ) {
#if DEBUG
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif

			var p = reader.Peek();
			if ( -1 == p ) {
				return null;
			}
			var c = System.Convert.ToChar( reader.Read() );
			if ( escape.HasValue && escape.Value.Equals( c ) ) {
				p = reader.Peek();
				if ( -1 == p ) {
					return null;
				}
				return System.Convert.ToChar( reader.Read() );
			}
			if ( @break.Equals( c ) ) {
				if ( readNextOnBreak ) {
					p = reader.Peek();
					if ( -1 == p ) {
						return null;
					} else if ( @break.Equals( System.Convert.ToChar( p ) ) ) {
						return System.Convert.ToChar( reader.Read() );
					} else {
						reader.Read();
					}
				}
				return null;
			}
			return c;
		}

		protected sealed override void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<ColumnBase> fileColumns ) {
			if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}
			var qcs = this.QuoteCharString;
			var fss = this.FieldSeparatorString;
			var columnNameList = dbColumns.Select(
				x => x.ColumnName.Replace( qcs, qcs + qcs )
			);
			var rs = this.RecordSeparator;
			var list = this.ForceQuote
				? columnNameList.Select(
					x => qcs + x + qcs
				)
				: columnNameList.Select(
					x => ( x.Contains( qcs ) || x.Contains( fss ) || ( !System.String.IsNullOrEmpty( rs ) && x.Contains( rs ) ) )
						? qcs + x + qcs
						: x
				)
			;
			writer.Write( System.String.Join( fss, list ) );
			this.EolWriter( writer );
		}
		protected sealed override System.String GetRow( System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			var qcs = this.QuoteCharString;
			var fss = this.FieldSeparatorString;
			var valueList = columns.Select(
				x => this.GetColumn( formatMap[ x ], x, row ).Replace( qcs, qcs + qcs )
			);
			var rs = this.RecordSeparator;
			var list = this.ForceQuote
				? valueList.Select(
					x => qcs + x + qcs
				)
				: valueList.Select(
					x => ( x.Contains( qcs ) || x.Contains( fss ) || ( !System.String.IsNullOrEmpty( rs ) && x.Contains( rs ) ) )
						? qcs + x + qcs
						: x
				)
			;
			return System.String.Join( fss, list );
		}
		#endregion methods

	}

}