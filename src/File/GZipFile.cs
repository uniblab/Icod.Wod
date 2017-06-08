using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"gzipFile",
		Namespace = "http://Icod.Wod"
	)]
	public class GZipFile : FileOperationBase {

		#region fields
		private FileDescriptor myDestination;
		private System.Boolean myDelete;
		private System.IO.Compression.CompressionMode myCompressionMode;
		#endregion fields


		#region .ctor
		public GZipFile() : base() {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
		}
		public GZipFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false 
		)]
		public virtual FileDescriptor Destination {
			get {
				return myDestination;
			}
			set {
				myDestination = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"delete",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public virtual System.Boolean Delete {
			get {
				return myDelete;
			}
			set {
				myDelete = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"compressionMode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.IO.Compression.CompressionMode.Decompress )]
		public virtual System.IO.Compression.CompressionMode CompressionMode {
			get {
				return myCompressionMode;
			}
			set {
				myCompressionMode = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder order ) {
			this.WorkOrder = order;
			this.Destination.WorkOrder = order;
			System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase> action = null;
			switch ( this.CompressionMode ) {
				case System.IO.Compression.CompressionMode.Decompress :
					action = this.Decompress;
					break;
				case System.IO.Compression.CompressionMode.Compress :
					action = this.Compress;
					break;
				default :
					throw new System.InvalidOperationException();
			}

			var dest = this.Destination.GetFileHandler( order );
			var source = this.GetFileHandler( order );
			System.String file;
			var files = source.ListFiles();
			foreach ( var fe in files ) {
				file = fe.File;
				action( source, file, dest );
			}
			if ( this.Delete ) {
				foreach ( var fe in files ) {
					source.DeleteFile( fe.File );
				}
			}
		}
		private void Decompress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( "sourceFilePathName" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			var dfd = dest.FileDescriptor;
			using ( var reader = source.OpenReader( sourceFilePathName ) ) {
				using ( var gzip = new System.IO.Compression.GZipStream( reader, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						gzip.CopyTo( buffer );
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private void Compress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( "sourceFilePathName" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			var dfd = dest.FileDescriptor;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var gzip = new System.IO.Compression.GZipStream( buffer, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var reader = source.OpenReader( sourceFilePathName ) ) {
						reader.CopyTo( gzip );
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private System.String GetDestinatonFileName( System.String source ) {
			if ( System.String.IsNullOrEmpty( source ) ) {
				throw new System.ArgumentNullException( "source" );
			}
			System.String fn;
			switch ( this.CompressionMode ) {
				case System.IO.Compression.CompressionMode.Decompress :
					fn = System.IO.Path.GetFileNameWithoutExtension( source );
					break;
				case System.IO.Compression.CompressionMode.Compress :
					fn = System.IO.Path.GetFileName( source ) + ".gzip";
					break;
				default :
					throw new System.InvalidOperationException();
			}
			return fn;
		}
		#endregion methods

	}

}