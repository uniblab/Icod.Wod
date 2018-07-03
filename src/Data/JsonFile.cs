﻿using System;
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
				var assemblies = this.LoadAssemblies( this.WorkOrder );
				var list = this.ExecuteMap( this.ReadFile( filePathName, assemblies ), assemblies );
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
		private System.Object ReadFile( System.String fileName, System.Collections.Generic.IEnumerable<System.Reflection.Assembly> collection ) {
			using ( var fileStream = System.IO.File.Open( fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				using ( var streamReader = new System.IO.StreamReader( fileStream, System.Text.Encoding.ASCII, true, this.BufferLength, true ) ) {
					var engine = new Newtonsoft.Json.JsonSerializer();
					using ( var reader = new Newtonsoft.Json.JsonTextReader( streamReader ) ) {
						var output = engine.Deserialize( reader, this.GetTypeFromName( this.TypeName, collection ) );
						return output;
					}
				}
			}
		}
		private System.Collections.Generic.IEnumerable<System.Reflection.Assembly> LoadAssemblies( WorkOrder workOrder ) {
			System.Collections.Generic.ICollection<System.Reflection.Assembly> output = new System.Collections.Generic.List<System.Reflection.Assembly>();
			foreach ( var fd in ( this.Assemblies ?? new File.FileDescriptor[ 0 ] ) ) {
				var handler = fd.GetFileHandler( workOrder );
				foreach ( var file in handler.ListFiles() ) {
					using ( var reader = handler.OpenReader( file.File ) ) {
						using ( var raw = new System.IO.MemoryStream() ) {
							reader.CopyTo( raw );
							output.Add( System.Reflection.Assembly.Load( raw.ToArray() ) );
						}
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}