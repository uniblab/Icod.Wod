// Copyright (C) 2025  Timothy J. Bruce

using System;
using System.Runtime.CompilerServices;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"deflateFile",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class DeflateFile : BinaryCompressedFileOperationBase {

		#region .ctor
		static DeflateFile() {
			var compressorMap = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.IO.Stream, System.IO.Stream>
			>( 2 );
			compressorMap.Add(
				System.IO.Compression.CompressionMode.Compress,
				x => (System.IO.Stream)System.Activator.CreateInstance(
					typeof( System.IO.Compression.DeflateStream ), 
					new System.Object[ 3 ] { x, System.IO.Compression.CompressionMode.Compress, true }
				)
			);
			compressorMap.Add(
				System.IO.Compression.CompressionMode.Decompress,
				x => (System.IO.Stream)System.Activator.CreateInstance(
					typeof( System.IO.Compression.DeflateStream ),
					new System.Object[ 3 ] { x, System.IO.Compression.CompressionMode.Decompress, true }
				)
			);
			RegisterCompressorMap( typeof( DeflateFile ), compressorMap );

			var fileNameMap = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.String, System.String>
			>( 2 );
			fileNameMap.Add(
				System.IO.Compression.CompressionMode.Compress,
				AddExtension
			);
			fileNameMap.Add(
				System.IO.Compression.CompressionMode.Decompress,
				PruneExtension
			);
			RegisterFileNameMap( typeof( DeflateFile ), fileNameMap );


			var actionMap = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Action<FileHandlerBase, System.String, FileHandlerBase>
			>( 2 );
			actionMap.Add(
				System.IO.Compression.CompressionMode.Compress,
				( source, sourceFilePathName, dest ) => Compress(
					source, sourceFilePathName, dest,
					fileNameMap[ System.IO.Compression.CompressionMode.Compress ]( sourceFilePathName ),
					compressorMap[ System.IO.Compression.CompressionMode.Compress ]
				)
			);
			actionMap.Add(
				System.IO.Compression.CompressionMode.Decompress,
				( source, sourceFilePathName, dest ) => Decompress(
					source, sourceFilePathName, dest,
					fileNameMap[ System.IO.Compression.CompressionMode.Decompress ]( sourceFilePathName ),
					compressorMap[ System.IO.Compression.CompressionMode.Decompress ]
				)
			);
			RegisterActionMap( typeof( DeflateFile ), actionMap );
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
				action = GetActionMap( this.GetType(), cm );
			} catch ( System.Exception e ) {
				throw new System.InvalidOperationException(
					System.String.Format( "The specified compressionMode, {0}, is not supported.", cm ),
					e
				);
			}

			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			var files = source.ListFiles();
			System.Threading.Tasks.Parallel.ForEach( files, fe => {
				var file = fe.File;
				action( source, file, dest );
			} );
			if ( this.Delete ) {
				System.Threading.Tasks.Parallel.ForEach( files, fe => {
					source.DeleteFile( fe.File );
				} );
			}
		}
		#endregion methods


		#region static methods
		protected static System.String PruneExtension( System.String sourceFileName ) {
			return System.IO.Path.GetFileNameWithoutExtension( sourceFileName );
		}
		protected static System.String AddExtension( System.String sourceFilename ) {
			return System.IO.Path.GetFileName( sourceFilename ) + ".gzip";
		}
		#endregion static methods

	}

}
