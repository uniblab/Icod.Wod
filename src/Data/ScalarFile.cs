// Copyright (C) 2025  Timothy J. Bruce
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
		#endregion .ctor


		#region methods
		protected sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			workOrder = workOrder?? throw new System.ArgumentNullException( nameof( workOrder ) );
#endif
			var cols = ( columns ?? System.Array.Empty<System.Data.DataColumn>() );
			if ( this.WriteIfEmpty ) {
				if ( !cols.Any() ) {
					throw new System.ArgumentNullException( nameof( columns ) );
				}
			} else if (
				( !( rows ?? System.Array.Empty<System.Data.DataRow>() ).Any() )
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
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
			if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( nameof( filePathName ) );
			}

			var table = new System.Data.DataTable();
			try {
				var col = this.Columns.FirstOrDefault();
				table.Columns.Add( new System.Data.DataColumn {
					AllowDBNull = true,
					DataType = typeof( System.String )
				} );
				if ( 0 < this.Columns.Length ) {
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
