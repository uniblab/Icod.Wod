// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( TextFileBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( JsonFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( XmlFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ScalarFile ) )]
	public abstract class DataFileBase : Icod.Wod.File.FileBase, ITableDestination, ITableSource {

		#region fields
		protected const System.Char SpaceChar = ' ';

		private ColumnBase[] myColumns;
		private System.Boolean myAppend;
		#endregion fields


		#region .ctor
		protected DataFileBase() : base() {
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
		public virtual void WriteRecords( Icod.Wod.WorkOrder workOrder, ITableSource source ) {
			this.WriteRecords( workOrder, source.ReadTables( workOrder ).OfType<System.Data.DataTable>() );
		}
		protected virtual void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataTable> source ) {
			if ( ( source is null ) || !source.Any() ) {
				if ( this.WriteIfEmpty ) {
					throw new System.ArgumentNullException( nameof( source ) );
				} else {
					return;
				}
			}
			using ( var table = source.FirstOrDefault() ) {
				this.WriteRecords( workOrder, table );
			}
		}
		protected virtual void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Data.DataTable source ) {
			if ( source is null ) {
				if ( this.WriteIfEmpty ) {
					throw new System.ArgumentNullException( nameof( source ) );
				} else {
					return;
				}
			}
			workOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );

			this.WriteRecords( workOrder, source.Columns.OfType<System.Data.DataColumn>(), source.Rows.OfType<System.Data.DataRow>() );
		}
		protected abstract void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows );

		protected System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> BuildFormatMap( System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns ) {
			if ( ( dbColumns is null ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( dbColumns ) );
			}
			var output = new System.Collections.Generic.Dictionary<System.Data.DataColumn, ColumnBase>();
			var cols = this.Columns ?? System.Array.Empty<ColumnBase>();

			foreach ( var dbCol in dbColumns ) {
				output.Add( dbCol, cols.FirstOrDefault(
					x => x.Name.Equals( dbCol.ColumnName, System.StringComparison.OrdinalIgnoreCase )
				) ?? new TextFileColumn() { Name = dbCol.ColumnName } );
			}
			foreach ( var missing in dbColumns.Where(
				x => !output.ContainsKey( x )
			).ToArray() ) {
				output.Add( missing, new TextFileColumn() { Name = missing.ColumnName } );
			}
			return output;
		}
		protected void WriteFile( System.IO.Stream stream ) {
			stream = stream ?? throw new System.ArgumentNullException( nameof( stream ) );
			var handler = this.GetFileHandler( this.WorkOrder );
			var dfpn = handler.PathCombine( this.ExpandedPath, this.ExpandedName );
			_ = stream.Seek( 0, System.IO.SeekOrigin.Begin );
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
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
			if ( Icod.Wod.File.FileType.Directory == file.FileType ) {
				throw new System.InvalidOperationException();
			}
			return new System.IO.StreamReader( this.GetFileHandler( this.WorkOrder ).OpenReader( file.File ), this.GetEncoding(), true, this.BufferLength );
		}
		protected abstract System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file );
		public System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			foreach ( var file in this.GetFiles() ) {
				using ( var stream = this.OpenReader( file ) ) {
					using ( var table = this.ReadFile( file.File, stream ) ) {
						yield return table;
					}
				}
			}
		}
		#endregion

		#region static methods

		protected static void AddFileColumns( System.Data.DataTable table, System.String filePathName ) {
			if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( nameof( filePathName ) );
			}
			table = table ?? throw new System.ArgumentNullException( nameof( table ) );

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

			table.TableName = fileName;
		}
		#endregion static methods

	}

}
