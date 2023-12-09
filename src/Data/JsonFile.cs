// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"jsonFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class JsonFile : DataFileBase {

		#region .ctor
		public JsonFile() : base() {
		}
		#endregion .ctor


		#region methods
		protected sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			if ( workOrder is null ) {
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

			var collection = new System.Collections.Generic.List<System.Collections.Generic.IDictionary<System.String, System.Object>>();
			var keys = columns.Select(
				x => x.ColumnName
			);
			System.Collections.Generic.IDictionary<System.String, System.Object> record = null;
			foreach ( var row in rows ) {
				record = new System.Collections.Generic.Dictionary<System.String, System.Object>( System.StringComparer.OrdinalIgnoreCase );
				foreach ( var key in keys ) {
					record.Add( key, row[ key ] );
				}
				collection.Add( record );
			}

			if ( collection.Any() || this.WriteIfEmpty ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength, true ) ) {
						new Newtonsoft.Json.JsonSerializer().Serialize( writer, collection, typeof( System.Collections.Generic.IEnumerable<System.Collections.Generic.IDictionary<System.String, System.Object>> ) );
						writer.Flush();
					}
					buffer.Flush();
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					this.WriteFile( buffer );
				}
			}
		}

		protected sealed override System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file ) {
			if ( file is null ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( "filePathName" );
			}

			System.Data.DataTable table;
			try {
				var set = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataSet>( file.ReadToEnd() );
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
