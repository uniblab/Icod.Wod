// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"deflateFile",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class DeflateFile : BinaryCompressedFileOperationBase {

		#region fields
		private static readonly System.Collections.Generic.Dictionary<
			System.IO.Compression.CompressionMode,
			System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase>
		> theAction;
		#endregion fields


		#region .ctor
		static DeflateFile() {
			theAction = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase>
			>( 2 );
			theAction.Add(
				System.IO.Compression.CompressionMode.Compress, 
				Compress
			);
			theAction.Add(
				System.IO.Compression.CompressionMode.Decompress,
				Decompress
			);
		}

		public DeflateFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			this.Destination.WorkOrder = workOrder;
			System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase> action;
			var cm = this.CompressionMode;
			try {
				action = theAction[ cm ];
			} catch ( System.Exception e ) {
				throw new System.InvalidOperationException( 
					System.String.Format( "The specified compressionMode, {0}, is not supported.", cm ), 
					e 
				);
			}

			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
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
		#endregion methods


		#region static methods
		private static void Decompress( 
			Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest 
		) {
#if DEBUG
			dest = dest ?? throw new System.ArgumentNullException( nameof( dest ) );
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
#endif
			if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( sourceFilePathName ) );
			}
			var dfd = dest.FileDescriptor;
			using ( var reader = source.OpenReader( sourceFilePathName ) ) {
				using ( var worker = new System.IO.Compression.DeflateStream( reader, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						worker.CopyTo( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = GetDestinatonFileName( sourceFilePathName, System.IO.Compression.CompressionMode.Decompress );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private static void Compress( 
			Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest 
		) {
#if DEBUG
			dest = dest ?? throw new System.ArgumentNullException( nameof( dest ) );
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
#endif
			if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( sourceFilePathName ) );
			}
			var dfd = dest.FileDescriptor;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var worker = new System.IO.Compression.DeflateStream( buffer, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var reader = source.OpenReader( sourceFilePathName ) ) {
						reader.CopyTo( worker );
						worker.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = GetDestinatonFileName( sourceFilePathName, System.IO.Compression.CompressionMode.Compress );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private static System.String GetDestinatonFileName( System.String source, System.IO.Compression.CompressionMode compressionMode ) {
#if DEBUG
			if ( System.String.IsNullOrEmpty( source ) ) {
				throw new System.ArgumentNullException( nameof( source ) );
			}
#endif
			System.String fn;
			switch ( compressionMode ) {
				case System.IO.Compression.CompressionMode.Decompress:
					fn = System.IO.Path.GetFileNameWithoutExtension( source );
					break;
				case System.IO.Compression.CompressionMode.Compress:
					fn = System.IO.Path.GetFileName( source ) + ".gzip";
					break;
				default:
					throw new System.InvalidOperationException();
			}
			return fn;
		}
		#endregion static methods

	}

}
