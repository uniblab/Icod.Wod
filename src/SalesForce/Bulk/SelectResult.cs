// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class SelectResult : Data.ITableSource {

		#region fields
		private const System.Int32 EOF = -1;
		private const System.Char DQUOTE = '"';

		private System.String myLocator;
		#endregion fields


		#region .ctor
		public SelectResult() : base() {
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
				var output = ReadFile( reader, this.ColumnDelimiter, this.LineEnding );
				if ( ( this.AdditionalColumns ?? System.Array.Empty<System.Data.DataColumn>() ).Any() ) {
					foreach ( var column in this.AdditionalColumns ) {
						output.Columns.Add( column );
					}
				}
				return output;
			}
		}
		public System.Data.DataTable ReadFile( System.Char columDelimiter, System.String lineEnding ) {
			using ( var reader = new System.IO.StringReader( this.Body ) ) {
				return ReadFile( reader, columDelimiter, lineEnding );
			}
		}
		#endregion methods


		#region static methods
		private static System.Data.DataTable ReadFile( System.IO.StringReader file, System.Char columDelimiter, System.String lineEnding ) {
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );

			var lineNumber = 0;
			var table = new System.Data.DataTable();
			try {
				BuildTable( file, table, columDelimiter, lineEnding );
				foreach ( var record in ReadRecord( file, lineEnding ) ) {
					lineNumber++;
					var rowList = ReadColumns( record, columDelimiter, DQUOTE );
					try {
						_ = table.Rows.Add( rowList.ToArray() );
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
		private static void BuildTable( System.IO.StringReader file, System.Data.DataTable table, System.Char columDelimiter, System.String lineEnding ) {
#if DEBUG
			table = table ?? throw new System.ArgumentNullException( nameof( table ) );
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
#endif

			var headerLine = file.ReadLine( lineEnding, DQUOTE );
			foreach ( var column in ReadColumns( headerLine, columDelimiter, DQUOTE ) ) {
				table.Columns.Add( new System.Data.DataColumn( column, typeof( System.String ) ) );
			}
		}
		private static System.Collections.Generic.IEnumerable<System.String> ReadColumns( System.String line, System.Char fieldSeparator, System.Char quoteCharacter ) {
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
						_ = reader.Read();
						yield return ReadQuotedTextCell( reader, fieldSeparator, quoteCharacter );
					} else if ( fieldSeparator.Equals( c ) ) {
						_ = reader.Read();
						yield return null;
					} else {
						yield return ReadPlainTextCell( reader, fieldSeparator );
					}
				} while ( true );
			}

		}
		private static System.String ReadPlainTextCell( System.IO.StringReader reader, System.Char fieldSeparator ) {
			reader = reader ?? throw new System.ArgumentNullException( nameof( reader ) );

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
		private static System.String ReadQuotedTextCell( System.IO.StringReader reader, System.Char fieldSeparator, System.Char quoteCharacter ) {
			reader = reader ?? throw new System.ArgumentNullException( nameof( reader ) );

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
						_ = reader.Read();
						cell = cell.Append( c );
					} else if ( fieldSeparator.Equals( c ) ) {
						_ = reader.Read();
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
		private static System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StringReader file, System.String lineEnding ) {
#if DEBUG
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
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
		#endregion

	}

}
