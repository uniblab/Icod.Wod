using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DelimitedFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FixedWidthFile ) )]
	public abstract class FileBase : Icod.Wod.File.FileDescriptor, ITableDestination, ITableSource {

		#region fields
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;
		private const System.Char SpaceChar = ' ';

		private System.String myCodePage;
		private System.Boolean myHasHeader;
		private TextFileColumn[] myColumns;
		private System.Int32 myBufferLength;
		private System.Boolean myAppend;
		private System.Int32 mySkip;
		private System.Boolean myWriteIfEmpty;
		private System.String myRecordSeparator;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		private System.Boolean myConvertEmptyStringToNull;
		private System.Boolean myTrimValues;
		#endregion fields


		#region .ctor
		static FileBase() {
			theEmptyEolWriter = x => {};
		}

		protected FileBase() : base() {
			myCodePage = "windows-1252";
			myHasHeader = true;
			myColumns = null;
			myBufferLength = 16384;
			myAppend = false;
			mySkip = 0;
			myWriteIfEmpty = false;
			myRecordSeparator = "\r\n";
			myEolWriter = theEmptyEolWriter;
			myConvertEmptyStringToNull = true;
			myTrimValues = true;
		}
		protected FileBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myHasHeader = true;
			myColumns = null;
			myBufferLength = 16384;
			myAppend = false;
			mySkip = 0;
			myWriteIfEmpty = false;
			myRecordSeparator = "\r\n";
			myEolWriter = theEmptyEolWriter;
			myConvertEmptyStringToNull = true;
			myTrimValues = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "windows-1252" )]
		public System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"hasHeader",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean HasHeader {
			get {
				return myHasHeader;
			}
			set {
				myHasHeader = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			"columns",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"column",
			typeof( TextFileColumn ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual TextFileColumn[] Columns {
			get {
				return myColumns;
			}
			set {
				myColumns = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"bufferLength",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 16384 )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"append",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Append {
			get {
				return myAppend;
			}
			set {
				myAppend = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"skipCount",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Skip {
			get {
				return mySkip;
			}
			set {
				if ( value < 0 ) {
					throw new System.ArgumentOutOfRangeException( "value", "Parameter cannot be negative." );
				}
				mySkip = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"writeIfEmpty",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean WriteIfEmpty {
			get {
				return myWriteIfEmpty;
			}
			set {
				myWriteIfEmpty = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		protected System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? theEmptyEolWriter;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "\r\n" )]
		public System.String RecordSeparator {
			get {
				return myRecordSeparator;
			}
			set {
				myRecordSeparator = value;
				myEolWriter = System.String.IsNullOrEmpty( value )
					? theEmptyEolWriter
					: x => x.Write( myRecordSeparator )
				;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"convertEmptyStringToNull",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean ConvertEmptyStringToNull {
			get {
				return myConvertEmptyStringToNull;
			}
			set {
				myConvertEmptyStringToNull = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"trimValues",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean TrimValues {
			get {
				return myTrimValues;
			}
			set {
				myTrimValues = value;
			}
		}
		#endregion properties


		#region methods
		public virtual System.Text.Encoding GetEncoding() {
			var cp = this.CodePage;
			System.Int32 cpNumber;
			if ( System.Int32.TryParse( cp, out cpNumber ) ) {
				return System.Text.Encoding.GetEncoding( cpNumber );
			} else {
				return System.Text.Encoding.GetEncoding( cp );
			}
		}

		public virtual void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			using ( var table = source.ReadTables( order ).FirstOrDefault() ) {
				var rows = table.Rows.OfType<System.Data.DataRow>();
				if ( !rows.Any() && !this.WriteIfEmpty ) {
					return;
				}
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength ) ) {
						var dbColumns = table.Columns.OfType<System.Data.DataColumn>();
						if ( this.HasHeader ) {
							this.WriteHeader( writer, dbColumns, this.Columns );
						}
						var formatMap = this.BuildFormatMap( dbColumns );
						foreach ( var row in rows ) {
							this.WriteRow( writer, formatMap, dbColumns, row );
						}
						writer.Flush();
						this.WriteFile( buffer );
					}
				}
			}
		}
		protected virtual System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> BuildFormatMap( System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns ) {
			if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			}
			var output = new System.Collections.Generic.Dictionary<System.Data.DataColumn, TextFileColumn>();
			var cols = this.Columns ?? new TextFileColumn[ 0 ];

			foreach ( var dbCol in dbColumns ) {
				output.Add( dbCol, cols.FirstOrDefault(
					x => x.Name.Equals( dbCol.ColumnName, System.StringComparison.OrdinalIgnoreCase )
				) ?? new TextFileColumn( dbCol.ColumnName ) );
			}
			return output;
		}
		protected virtual void WriteHeader( System.IO.StreamWriter writer, System.Data.DataTable table ) {
			if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			this.WriteHeader( writer, table.Columns.OfType<System.Data.DataColumn>(), this.Columns );
		}
		protected abstract void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns );
		protected virtual void WriteFile( System.IO.Stream stream ) {
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

		protected virtual void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			writer.WriteLine( GetRow( formatMap, columns, row ) );
		}
		protected virtual System.String GetRow( System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			return System.String.Join( System.String.Empty, columns.Select(
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
		protected virtual System.Collections.Generic.IEnumerable<System.String> GetColumns( System.Collections.Generic.IDictionary<System.Data.DataColumn, TextFileColumn> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( null == columns ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( null == formatMap ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			return columns.Select(
				x => this.GetColumn( 
					formatMap.ContainsKey( x )
						? formatMap[ x ] ?? new TextFileColumn( x.ColumnName )
						: new TextFileColumn( x.ColumnName )
					,
					x,
					row
				)
			);
		}
		protected virtual System.String GetColumn( TextFileColumn format, System.Data.DataColumn column, System.Data.DataRow row ) {
			if ( null == row ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( null == column ) {
				throw new System.ArgumentNullException( "column" );
			}


			if ( null == format ) {
				format = new TextFileColumn( column.ColumnName );
			}
			var s = System.String.Format( format.FormatString ?? "{0}", row[ column ] ?? System.String.Empty );
			if ( 0 < format.Length ) {
				var l = format.Length;
				var w = l - s.Length;
				s = s.PadLeft( w, SpaceChar );
			}
			return s;
		}

		public virtual System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder order ) {
			this.WorkOrder = order;
			System.String fileName;
			foreach ( var file in this.GetFiles() ) {
				fileName = System.IO.Path.GetFileName( file.File );
				using ( var stream = this.OpenReader( file ) ) {
					yield return this.ReadFile( fileName, stream );
				}
			}
		}

		protected virtual System.Collections.Generic.IEnumerable<Icod.Wod.File.FileEntry> GetFiles() {
			return this.GetFileHandler( this.WorkOrder ).ListFiles();
		}
		protected virtual System.IO.StreamReader OpenReader( Icod.Wod.File.FileEntry file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( Icod.Wod.File.FileType.Directory == file.FileType ) {
				throw new System.InvalidOperationException();
			}
			return new System.IO.StreamReader( this.GetFileHandler( this.WorkOrder ).OpenReader( file.File ), this.GetEncoding(), true, this.BufferLength );
		}

		protected virtual System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			var output = new System.Data.DataTable();
			foreach ( var column in this.BuildColumns( file ) ) {
				output.Columns.Add( column );
			}
			var fileNameColumn = new System.Data.DataColumn( "%wod:FileName%", typeof( System.String ) );
			fileNameColumn.AllowDBNull = false;
			fileNameColumn.ReadOnly = true;
			fileNameColumn.DefaultValue = fileName;
			output.Columns.Add( fileNameColumn );
			output.TableName = fileName;

			return output;
		}
		protected abstract System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StreamReader file );
		protected abstract System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file );
		protected abstract System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file );
		protected virtual System.Data.DataTable ReadFile( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			System.Data.DataTable table = null;
			try {
				table = this.BuildTable( fileName, file );
				if ( !file.EndOfStream ) {
					System.Int32 s = 0;
					var skip = this.Skip;
					while ( ( ++s < skip ) && !file.EndOfStream ) {
						this.ReadRecord( file );
					}
					while ( !file.EndOfStream ) {
						this.ReadRecord( table, file );
					}
				}
			} catch ( System.Exception e ) {
				if ( !e.Data.Contains( "%wod:FileName%" ) ) {
					e.Data.Add( "%wod:FileName%", fileName );
				} else {
					e.Data[ "%wod:FileName%" ] = fileName;
				}
				throw;
			}
			return table;
		}
		#endregion methods

	}

}