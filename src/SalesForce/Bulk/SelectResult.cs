using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class SelectResult {

		#region fields
		private static readonly System.Func<System.Text.StringBuilder, System.String> theValueToString;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theTrimToNullReader;

		private const System.Char COMMA = ',';
		private const System.String CRLF = "\r\n";
		private const System.Char DQUOTE = '"';

		private System.String myLocator;
		private System.Func<System.Text.StringBuilder, System.String> myColumnReader;
		#endregion fields


		#region .ctor
		static SelectResult() {
			theValueToString = a => ( null == a )
				? null
				: ( System.DBNull.Value.Equals( a ) )
					? null
					: a.ToString()
			;
			theTrimToNullReader = a => theValueToString( a ).TrimToNull();
		}

		public SelectResult() : base() {
			myColumnReader = theTrimToNullReader;
		}
		#endregion .ctor


		#region properties
		public System.String Locator {
			get {
				return myLocator;
			}
			set {
				value = value.TrimToNull();
				if ( "null".Equals( value ) ) {
					value = null;
				}
				myLocator = value;
			}
		}
		public System.Int32 RecordCount {
			get;
			set;
		}

		public System.String Body {
			get;
			set;
		}

		[System.Xml.Serialization.XmlIgnore]
		private System.Func<System.Text.StringBuilder, System.String> ColumnReader {
			get {
				return myColumnReader ?? theTrimToNullReader;
			}
		}
		#endregion properties


		#region methods
		public System.Data.DataTable ReadFile() {
			using ( var reader = new System.IO.StringReader( this.Body ) ) {
				return this.ReadFile( reader );
			}
		}
		protected System.Data.DataTable ReadFile( System.IO.StringReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}

			var table = new System.Data.DataTable();
			try {
				this.BuildTable( file, table );
				while ( -1 != file.Peek() ) {
					this.ReadRecord( table, file );
				}
			} catch ( System.Exception e ) {
				throw;
			}
			return table;
		}
		private void BuildTable( System.IO.StringReader file, System.Data.DataTable table ) {
#if DEBUG
			if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			foreach ( var column in this.BuildColumns( file ) ) {
				table.Columns.Add( column );
			}
		}
		protected System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StringReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
			return this.ReadHeaderLine( file ).Select(
				x => new System.Data.DataColumn( x, typeof( System.String ) )
			);
		}
		protected System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StringReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			}

			var row = this.ReadRecord( file );
			var rowList = row.ToArray();
			return table.Rows.Add( rowList );
		}
		private System.Collections.Generic.IEnumerable<System.String> ReadHeaderLine( System.IO.StringReader file ) {
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif
			var output = this.ReadRecord( file );
			return output;
		}
		protected System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StringReader file ) {
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			var line = file.ReadLine( CRLF, DQUOTE );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String column;
				var reading = true;
				var qc = DQUOTE;
				do {
					i = reader.Peek();
					if ( -1 == i ) {
						reading = false;
						break;
					}
					c = System.Convert.ToChar( i );
					if ( qc.Equals( c ) ) {
						reader.Read();
						column = this.ReadColumn( reader, null, DQUOTE, true );
						yield return column;
					} else {
						column = this.ReadColumn( reader, null, COMMA, false );
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
			do {
				ch = this.ReadChar( reader, null, @break, readNextOnBreak );
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
		#endregion methods

	}

}