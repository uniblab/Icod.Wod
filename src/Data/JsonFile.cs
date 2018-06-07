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

		public File.FileDescriptor[] Assemblies {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, ITableSource source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}

			var collection = new System.Collections.Generic.List<System.Collections.Generic.IDictionary<System.String, System.Object>>();
			foreach ( var table in source.ReadTables( workOrder ) ) {
				var rows = table.Rows.OfType<System.Data.DataRow>();
				if ( !rows.Any() && !this.WriteIfEmpty ) {
					return;
				}
				var keys = table.Columns.OfType<System.Data.DataColumn>().Select(
					x => x.ColumnName
				);
				System.Collections.Generic.IDictionary<System.String, System.Object> record = null;
				foreach ( var row in table.Rows.OfType<System.Data.DataRow>() ) {
					record = new System.Collections.Generic.Dictionary<System.String, System.Object>( System.StringComparer.OrdinalIgnoreCase );
					foreach ( var key in keys ) {
						record.Add( key, row[ key ] );
					}
					collection.Add( record );
				}
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

			this.LoadAssemblies( this.WorkOrder );
			var table = new System.Data.DataTable();
			try {
				var list = this.ExecuteMap( this.ReadFile( filePathName ) );
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
		private System.Collections.Generic.IEnumerable<System.Object> ExecuteMap( System.Object obj ) {
			var t = System.Type.GetType( this.MapWith, true, false );
			var mapper = (IMapWith)System.Activator.CreateInstance( t );
			return mapper.ExecuteMap( obj );
		}
		private System.Object ReadFile( System.String fileName ) {
			using ( var fileStream = System.IO.File.Open( fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				using ( var streamReader = new System.IO.StreamReader( fileStream, System.Text.Encoding.ASCII, true, this.BufferLength, true ) ) {
					var typeName = this.TypeName.TrimToNull();
					var engine = new Newtonsoft.Json.JsonSerializer();
					using ( var reader = new Newtonsoft.Json.JsonTextReader( streamReader ) ) {
						return (  System.String.IsNullOrEmpty( typeName ) )
							? engine.Deserialize( reader )
							: engine.Deserialize( reader, System.Type.GetType( typeName, true, false ) )
						;
					}
				}
			}
		}
		private void LoadAssemblies( WorkOrder workOrder ) {
			foreach ( var fd in ( this.Assemblies ?? new File.FileDescriptor[ 0 ] ) ) {
				var handler = fd.GetFileHandler( workOrder );
				foreach ( var file in handler.ListFiles() ) {
					using ( var reader = handler.OpenReader( file.File ) ) {
						using ( var raw = new System.IO.MemoryStream() ) {
							reader.CopyTo( raw );
							System.Reflection.Assembly.Load( raw.ToArray() );
						}
					}
				}
			}
		}
		#endregion methods

	}

}