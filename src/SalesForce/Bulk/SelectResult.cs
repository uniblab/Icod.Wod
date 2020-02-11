using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class SelectResult : Data.ITableSource {

		#region fields
		private const System.Int32 EOF = -1;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theValueToString;
		private static readonly System.Func<System.Text.StringBuilder, System.String> theTrimToNullReader;

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
		public System.Char ColumnDelimiter {
			get;
			set;
		}
		[System.Xml.Serialization.XmlIgnore]
		public System.String LineEnding {
			get;
			set;
		}

		[System.Xml.Serialization.XmlIgnore]
		private System.Func<System.Text.StringBuilder, System.String> ColumnReader {
			get {
				return myColumnReader ?? theTrimToNullReader;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Collections.Generic.IEnumerable<System.Data.DataColumn> AdditionalColumns {
			get;
			set;
		}
		#endregion properties


		#region methods
		public System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder workOrder ) {
			yield return this.ReadFile();
		}

		public System.Data.DataTable ReadFile() {
			using ( var reader = new System.IO.StringReader( this.Body ) ) {
				var output = this.ReadFile( reader, this.ColumnDelimiter, this.LineEnding );
				if ( ( this.AdditionalColumns ?? new System.Data.DataColumn[ 0 ] ).Any() ) {
					foreach ( var column in this.AdditionalColumns ) {
						output.Columns.Add( column );
					}
				}
				return output;
			}
		}
		public System.Data.DataTable ReadFile( System.Char columDelimiter, System.String lineEnding ) {
			using ( var reader = new System.IO.StringReader( this.Body ) ) {
				return this.ReadFile( reader, columDelimiter, lineEnding );
			}
		}
		protected System.Data.DataTable ReadFile( System.IO.StringReader file, System.Char columDelimiter, System.String lineEnding ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}

			var lineNumber = 0;
			var table = new System.Data.DataTable();
			try {
				this.BuildTable( file, table, columDelimiter, lineEnding );
				foreach ( var record in this.ReadRecord( file, lineEnding ) ) {
					lineNumber++;
					var rowList = this.ReadColumns( record, columDelimiter, DQUOTE );
					try {
						table.Rows.Add( rowList.ToArray() );
					} catch {
						throw;
					}
				}
			} catch ( System.Exception e ) {
				e.Data.Add( "lineNumber", lineNumber );
				throw;
			}
			return table;
		}
		private void BuildTable( System.IO.StringReader file, System.Data.DataTable table, System.Char columDelimiter, System.String lineEnding ) {
#if DEBUG
			if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			var headerLine = file.ReadLine( lineEnding, DQUOTE );
			foreach ( var column in this.ReadColumns( headerLine, columDelimiter, DQUOTE ) ) {
				table.Columns.Add( new System.Data.DataColumn( column, typeof( System.String ) ) );
			}
		}
		protected System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StringReader file, System.String lineEnding ) {
#if DEBUG
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			System.String line;
			while ( EOF != file.Peek() ) {
				line = file.ReadLine( lineEnding, DQUOTE ).TrimToNull();
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
			System.Int32 p;
			System.Char c;
			using ( var reader = new System.IO.StringReader( line ) ) {
				do {
					p = reader.Peek();
					if ( EOF == p ) {
						yield break;
					}
					c = System.Convert.ToChar( p );
					if ( quoteCharacter.Equals( c ) ) {
						reader.Read();
						yield return this.ReadQuotedTextCell( reader, fieldSeparator, quoteCharacter );
					} else if ( fieldSeparator.Equals( c ) ) {
						reader.Read();
						yield return null;
					} else {
						yield return this.ReadPlainTextCell( reader, fieldSeparator );
					}
				} while ( true );
			}

		}

		private System.String ReadPlainTextCell( System.IO.StringReader reader, System.Char fieldSeparator ) {
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}

			var cell = new System.Text.StringBuilder();
			System.Int32 p;
			System.Char c;
			do {
				p = reader.Read();
				if ( EOF == p ) {
					break;
				}
				c = System.Convert.ToChar( p );
				if ( fieldSeparator.Equals( c ) ) {
					break;
				}
				cell = cell.Append( c );
			} while ( true );

			return cell.ToString().TrimToNull();
		}
		private System.String ReadQuotedTextCell( System.IO.StringReader reader, System.Char fieldSeparator, System.Char quoteCharacter ) {
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}

			var cell = new System.Text.StringBuilder();
			System.Int32 p;
			System.Char c;
			do {
				p = reader.Read();
				if ( EOF == p ) {
					break;
				}
				c = System.Convert.ToChar( p );
				if ( quoteCharacter.Equals( c ) ) {
					p = reader.Peek();
					if ( EOF == p ) {
						break;
					}
					c = System.Convert.ToChar( p );
					if ( quoteCharacter.Equals( c ) ) {
						reader.Read();
						cell = cell.Append( c );
					} else if ( fieldSeparator.Equals( c ) ) {
						reader.Read();
						break;
					} else {
						throw new System.InvalidOperationException();
					}
				} else {
					cell = cell.Append( c );
				}
			} while ( true );

			return cell.ToString().TrimToNull();
		}
		#endregion methods

	}

}