// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"xmlFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class XmlFile : DataFileBase {

		#region .ctor
		public XmlFile() : base() {
		}
		#endregion .ctor


		#region methods
		protected sealed override void WriteRecords( WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
#endif
			using ( var set = new System.Data.DataSet() ) {
				var table = new System.Data.DataTable( "table1" );
				if ( rows.Any() ) {
					var r = rows.First();
					if ( r.Table is object ) {
						table.TableName = r.Table.TableName.TrimToNull() ?? "table1";
					}
				}
				foreach ( var column in columns ) {
					table.Columns.Add( column.ColumnName );
				}
				foreach ( var row in rows ) {
					table.Rows.Add( row.ItemArray );
				}
				set.Tables.Add( table );
				using ( var buffer = new System.IO.MemoryStream() ) {
					set.WriteXml( buffer );
					buffer.Flush();
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					var handler = this.GetFileHandler( this.WorkOrder );
					var dfpn = handler.PathCombine( this.ExpandedPath, this.ExpandedName );
					handler.Overwrite( buffer, dfpn );
				}
			}
		}
		protected sealed override System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file ) {
#if DEBUG
			if ( file is null ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( "filePathName" );
			}
#endif
			System.Data.DataTable table = null;
			try {
				var set = new System.Data.DataSet();
				set.ReadXml( file );
				table = set.Tables[ 0 ];
				this.AddFileColumns( table, filePathName );
			} catch ( System.Exception e ) {
				if ( !e.Data.Contains( "%wod:FilePathName%" ) ) {
					e.Data.Add( "%wod:FilePathName%", filePathName );
				} else {
					e.Data[ "%wod:FilePathName%" ] = filePathName;
				}
				if ( !e.Data.Contains( "%wod:FileName%" ) ) {
					e.Data.Add( "%wod:FileName%", System.IO.Path.GetFileName( filePathName ) );
				} else {
					e.Data[ "%wod:FileName%" ] = System.IO.Path.GetFileName( filePathName );
				}
				throw;
			}
			return table;
		}
		#endregion methods

	}

}
