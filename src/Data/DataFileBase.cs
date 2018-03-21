using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( TextFileBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( JsonFile ) )]
	public abstract class DataFileBase : Icod.Wod.File.FileBase, ITableDestination, ITableSource {

		#region fields
		protected const System.Char SpaceChar = ' ';

		private ColumnBase[ ] myColumns;
		private System.Boolean myAppend;
		#endregion fields


		#region .ctor
		protected DataFileBase() : base() {
			myColumns = null;
			myAppend = false;
		}
		protected DataFileBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myColumns = null;
			myAppend = false;
		}
		#endregion .ctor


		#region properties
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
		public virtual ColumnBase[] Columns {
			get {
				return myColumns;
			}
			set {
				myColumns = value;
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
		#endregion properties


		#region methods
		public abstract void WriteRecords( Icod.Wod.WorkOrder workOrder, ITableSource source );
		protected System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> BuildFormatMap( System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns ) {
			if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			}
			var output = new System.Collections.Generic.Dictionary<System.Data.DataColumn, ColumnBase>();
			var cols = this.Columns ?? new TextFileColumn[ 0 ];

			foreach ( var dbCol in dbColumns ) {
				output.Add( dbCol, cols.FirstOrDefault(
					x => x.Name.Equals( dbCol.ColumnName, System.StringComparison.OrdinalIgnoreCase )
				) ?? new TextFileColumn( dbCol.ColumnName ) );
			}
			foreach ( var missing in dbColumns.Where(
				x => !output.ContainsKey( x )
			).ToArray() ) {
				output.Add( missing, new TextFileColumn( missing.ColumnName ) );
			}
			return output;
		}
		protected void WriteFile( System.IO.Stream stream ) {
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

		protected System.Collections.Generic.IEnumerable<Icod.Wod.File.FileEntry> GetFiles() {
			return this.GetFileHandler( this.WorkOrder ).ListFiles();
		}
		protected System.IO.StreamReader OpenReader( Icod.Wod.File.FileEntry file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( Icod.Wod.File.FileType.Directory == file.FileType ) {
				throw new System.InvalidOperationException();
			}
			return new System.IO.StreamReader( this.GetFileHandler( this.WorkOrder ).OpenReader( file.File ), this.GetEncoding(), true, this.BufferLength );
		}
		protected abstract System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file );
		public System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			foreach ( var file in this.GetFiles() ) {
				using ( var stream = this.OpenReader( file ) ) {
					yield return this.ReadFile( file.File, stream );
				}
			}
		}

		protected void AddFileColumns( System.Data.DataTable table, System.String filePathName ) {
			if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( "filePathName" );
			} else if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			}

			var fileName = System.IO.Path.GetFileName( filePathName );
			var fileNameColumn = new System.Data.DataColumn( "%wod:FileName%", typeof( System.String ) ) {
				AllowDBNull = false,
				ReadOnly = true,
				DefaultValue = fileName
			};
			table.Columns.Add( fileNameColumn );

			var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension( fileName );
			var fileNameWithoutExtensionColumn = new System.Data.DataColumn( "%wod:FileNameWithoutExtension%", typeof( System.String ) ) {
				AllowDBNull = false,
				ReadOnly = true,
				DefaultValue = fileNameWithoutExtension
			};
			table.Columns.Add( fileNameWithoutExtensionColumn );

			var filePathNameColumn = new System.Data.DataColumn( "%wod:FilePathName%", typeof( System.String ) ) {
				AllowDBNull = false,
				ReadOnly = true,
				DefaultValue = filePathName
			};
			table.Columns.Add( filePathNameColumn );

			var directoryName = System.IO.Path.GetDirectoryName( filePathName );
			var directoryNameColumn = new System.Data.DataColumn( "%wod:DirectoryName%", typeof( System.String ) ) {
				AllowDBNull = false,
				ReadOnly = true,
				DefaultValue = directoryName
			};
			table.Columns.Add( directoryNameColumn );

			table.TableName = fileName;
		}
		#endregion methods

	}

}