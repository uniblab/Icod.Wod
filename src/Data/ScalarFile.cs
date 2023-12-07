// Copyright 2022, Timothy J. Bruce
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"scalarFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class ScalarFile : DataFileBase {

		#region .ctor
		public ScalarFile() : base() {
		}
		public ScalarFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
#endif
			var cols = ( columns ?? new System.Data.DataColumn[ 0 ] );
			if ( this.WriteIfEmpty ) {
				if ( !cols.Any() ) {
					throw new System.ArgumentNullException( "columns" );
				}
			} else if (
				( !( rows ?? new System.Data.DataRow[ 0 ] ).Any() )
				|| ( !cols.Any() )
			) {
				return;
			}
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength ) ) {
					System.Object record;
					foreach ( var row in rows ) {
						record = row[ 0 ];
						if ( ( null != record ) && !System.DBNull.Value.Equals( record ) ) {
							writer.Write( record );
						}
					}
					writer.Write( this.RecordSeparator );
					writer.Flush();
					this.WriteFile( buffer );
				}
			}
		}

		protected sealed override System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( "filePathName" );
			}

			var table = new System.Data.DataTable();
			try {
				var col = this.Columns.FirstOrDefault();
				table.Columns.Add( new System.Data.DataColumn {
					AllowDBNull = true,
					DataType = typeof( System.String )
				} );
				if ( this.Columns.Any() ) {
					table.Columns[ 0 ].ColumnName = this.Columns.FirstOrDefault().Name;
				}
				var record = file.ReadLine( this.RecordSeparator );
				_ = table.Rows.Add( record );
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
