using System;
using System.Collections.Generic;
using System.IO;
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
		public JsonFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"typeName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String TypeName {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"mapWith",
			Namespace = "http://Icod.Wod"
		)]
		public System.String MapWith {
			get;
			set;
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "assemblies"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"assembly",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public File.FileDescriptor[] Assemblies {
			get;
			set;
		}
		#endregion properties


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
					buffer.Seek( 0, SeekOrigin.Begin );
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
				if ( System.String.IsNullOrEmpty( this.TypeName ) ) {
					this.ReadFileAsJson( file, table );
				} else {
					this.ReadFileWithMap( file, table );
				}
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
		private void ReadFileWithMap( System.IO.StreamReader file, System.Data.DataTable table ) {
			var assemblies = this.LoadAssemblies( this.WorkOrder );
			var list = this.ExecuteMap( this.ReadFile( file, assemblies ), assemblies );
			var pi = list.GetType().GetGenericArguments().Last().GetProperties();
			foreach ( var p in pi ) {
				table.Columns.Add( new System.Data.DataColumn( p.Name ) );
			}
			System.Data.DataRow row = null;
			foreach ( var record in list ) {
				row = table.NewRow();
				foreach ( var p in pi ) {
					row[ p.Name ] = p.GetValue( record );
				}
				table.Rows.Add( row );
			}
		}
		private void ReadFileAsJson( System.IO.StreamReader file, System.Data.DataTable table ) {
			var records = this.ReadFile( file );
			System.Collections.Generic.ICollection<ColumnBase> cols = this.Columns;
			if ( !( cols ?? new ColumnBase[ 0 ] ).Any() ) {
				cols = new System.Collections.Generic.List<ColumnBase>();
				dynamic record = records.First;
				foreach ( var key in ( record as System.Collections.Generic.IDictionary<System.String, Newtonsoft.Json.Linq.JToken> ).Where(
					x => !x.Value.HasValues
				).Select(
					x => x.Key
				) ) {
					cols.Add( new TextFileColumn( key ) );
				}
			}
			foreach ( var column in cols ) {
				var dataCol = new System.Data.DataColumn( column.Name );
				if ( 0 < column.Length ) {
					dataCol.MaxLength = column.Length;
				}
				table.Columns.Add( dataCol );
			}
			System.Data.DataRow row = null;
			var colNames = cols.Select(
				x => x.Name
			);
			foreach ( dynamic record in records ) {
				row = table.NewRow();
				foreach ( var colName in colNames ) {
					row[ colName ] = record[ colName ].Value;
				}
				table.Rows.Add( row );
			}
		}
		private System.Collections.Generic.IEnumerable<System.Object> ExecuteMap( System.Object obj, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> collection ) {
			var t = this.GetTypeFromName( this.MapWith, collection );
			var mapper = (Icod.Wod.Map.IMapWith)System.Activator.CreateInstance( t );
			var output = mapper.ExecuteMap( obj );
			return output;
		}
		private System.Type GetJsonType( System.Collections.Generic.IEnumerable<System.Reflection.Assembly> collection ) {
			return this.GetTypeFromName( this.TypeName, collection );
		}
		private System.Type GetTypeFromName( System.String name, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> collection ) {
			return collection.Select(
				x => x.GetType( name, true, false )
			).Where(
				x => null != x
			).FirstOrDefault();
		}
		private System.Object ReadFile( System.IO.StreamReader file, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> collection ) {
			var engine = new Newtonsoft.Json.JsonSerializer();
			return engine.Deserialize( file, this.GetTypeFromName( this.TypeName, collection ) );
		}
		private Newtonsoft.Json.Linq.JObject ReadFile( System.IO.StreamReader file ) {
			return Newtonsoft.Json.Linq.JObject.Parse( file.ReadToEnd() );
		}
		private System.Collections.Generic.IEnumerable<System.Reflection.Assembly> LoadAssemblies( WorkOrder workOrder ) {
			System.Collections.Generic.ICollection<System.Reflection.Assembly> output = new System.Collections.Generic.List<System.Reflection.Assembly>();
			foreach ( var fd in ( this.Assemblies ?? new File.FileDescriptor[ 0 ] ) ) {
				var handler = fd.GetFileHandler( workOrder );
				foreach ( var file in handler.ListFiles() ) {
					using ( var reader = handler.OpenReader( file.File ) ) {
						using ( var raw = new System.IO.MemoryStream() ) {
							reader.CopyTo( raw );
							var a = System.Reflection.Assembly.Load( raw.ToArray() );
							output.Add( a );
						}
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}