using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class SelectResult {

		#region internal classes
		private enum ReadState {
			StartOfText,
			PlainText,
			QuotedText,
			EndOfLine,
			EndOfFile
		}
		#endregion internal classes

		#region fields
		private const System.Int32 EOF = -1;
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

			var lineNumber = 0;
			var table = new System.Data.DataTable();
			try {
				this.BuildTable( file, table );
				foreach ( var record in this.ReadRecord( file ) ) {
					lineNumber++;
					var rowList = this.ReadColumns( record, COMMA, DQUOTE );
					table.Rows.Add( rowList.ToArray() );
				}
			} catch ( System.Exception e ) {
				e.Data.Add( "lineNumber", lineNumber );
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

			var headerLine = file.ReadLine( CRLF, DQUOTE );
			foreach ( var column in this.ReadColumns( headerLine, COMMA, DQUOTE ) ) {
				table.Columns.Add( new System.Data.DataColumn( column, typeof( System.String ) ) );
			}
		}
		protected System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StringReader file ) {
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			System.String line;
			while ( EOF != file.Peek() ) {
				line = file.ReadLine( CRLF, DQUOTE ).TrimToNull();
				if ( !System.String.IsNullOrEmpty( line ) ) {
					yield return line;
				}
			}

			yield break;
		}
		protected System.Collections.Generic.IEnumerable<System.String> ReadColumns( System.String line, System.Char fieldSeparator, System.Char quoteCharacter ) {
			if ( System.String.IsNullOrEmpty( line ) ) {
				throw new System.ArgumentNullException( line );
			}


			var cell = new System.Text.StringBuilder();
			var readState = ReadState.PlainText;
			System.Int32 p;
			System.Char c;
			System.Char pc;
			using ( var reader = new System.IO.StringReader( line ) ) {
				while ( readState != ReadState.EndOfLine ) {
					p = reader.Peek();
					switch ( readState ) {
						case ReadState.PlainText:
							if ( EOF == p ) {
								readState = ReadState.EndOfFile;
								yield return cell.ToString();
								yield break;
							}
							c = System.Convert.ToChar( p );
							if ( fieldSeparator == c ) {
								reader.Read();
								yield return cell.ToString();
								cell = new System.Text.StringBuilder( cell.Capacity );
							} else if ( quoteCharacter == c ) {
								readState = ReadState.QuotedText;
								reader.Read();
							} else {
								cell = cell.Append( c );
							}
							break;
						case ReadState.QuotedText:
							if ( EOF == p ) {
								throw new System.InvalidOperationException();
							}
							c = System.Convert.ToChar( p );
							if ( quoteCharacter == c ) {
								p = reader.Peek();
								if ( EOF == p ) {
									throw new System.InvalidOperationException();
								}
								pc = System.Convert.ToChar( p );
								if ( fieldSeparator == pc ) {
									readState = ReadState.PlainText;
									reader.Read();
									yield return cell.ToString();
									cell = new System.Text.StringBuilder( cell.Capacity );
								} else {
									cell = cell.Append( pc );
									reader.Read();
								}
							} else {
								cell = cell.Append( c );
							}
							break;
						default:
							throw new System.InvalidOperationException();
					}
				}
			}

			yield break;
		}

		private System.String ReadPlainTextCell( System.IO.StringReader reader, System.Char fieldSeparator ) {
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}

			var readState = ReadState.PlainText;

			var cell = new System.Text.StringBuilder();
			System.Int32 p;
			System.Char c;
			while ( ReadState.PlainText == readState ) {
				p = reader.Read();
				if ( EOF == p ) {
					readState = ReadState.EndOfFile;
					break;
				}
				c = System.Convert.ToChar( p );
				if ( fieldSeparator.Equals( c ) ) {
					readState = ReadState.EndOfFile;
					break;
				}
				cell = cell.Append( c );
			}

			return cell.ToString();
		}
		private System.String ReadQuotedTextCell( System.IO.StringReader reader, System.Char quoteCharacter ) {
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}

			var readState = ReadState.QuotedText;

			var cell = new System.Text.StringBuilder();
			System.Int32 p;
			System.Char c;
			while ( ReadState.QuotedText == readState ) {
				p = reader.Read();
				if ( EOF == p ) {
					readState = ReadState.EndOfFile;
					break;
				}
				c = System.Convert.ToChar( p );
				if ( quoteCharacter.Equals( c ) ) {
					p = reader.Read();
					if ( EOF == p ) {
						throw new System.InvalidOperationException();
					}
					c = System.Convert.ToChar( p );
	
				} else {
					cell = cell.Append( c );
				}
			}

			return cell.ToString();
		}
		#endregion methods

	}

}