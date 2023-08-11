// Icod.Wod.dll is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

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
		public XmlFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override void WriteRecords( WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
#endif
			using ( var set = new System.Data.DataSet() ) {
				var table = new System.Data.DataTable( "table1" );
				if ( rows.Any() ) {
					var r = rows.First();
					if ( null != r.Table ) {
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
			if ( null == file ) {
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
