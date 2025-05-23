// Copyright (C) 2025  Timothy J. Bruce

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
			> ( 2 ) {
				{
					System.IO.Compression.CompressionMode.Compress,
					x => (System.IO.Stream)System.Activator.CreateInstance(
						typeof( System.IO.Compression.DeflateStream ),
						new System.Object[ 3 ] { x, System.IO.Compression.CompressionMode.Compress, true }
					)
				},
				{
					System.IO.Compression.CompressionMode.Decompress,
					x => (System.IO.Stream)System.Activator.CreateInstance(
						typeof( System.IO.Compression.DeflateStream ),
						new System.Object[ 3 ] { x, System.IO.Compression.CompressionMode.Decompress, true }
					)
				},          
			};
			RegisterCompressorMap( typeof( DeflateFile ), compressorMap );

			var fileNameMap = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.String, System.String>
			>( 2 ) {
				{
					System.IO.Compression.CompressionMode.Compress,
					AddExtension
				},
				{
					System.IO.Compression.CompressionMode.Decompress,
					PruneExtension
				},
			};
			RegisterFileNameMap( typeof( DeflateFile ), fileNameMap );
			var actionMap = new System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Action<FileHandlerBase, System.String, FileHandlerBase>
			>( 2 ) {
				{
					System.IO.Compression.CompressionMode.Compress,
					( source, sourceFilePathName, dest ) => Compress(
						source, sourceFilePathName, dest,
						fileNameMap[ System.IO.Compression.CompressionMode.Compress ]( sourceFilePathName ),
						compressorMap[ System.IO.Compression.CompressionMode.Compress ]
					)
				},
				{
					System.IO.Compression.CompressionMode.Decompress,
					( source, sourceFilePathName, dest ) => Decompress(
						source, sourceFilePathName, dest,
						fileNameMap[ System.IO.Compression.CompressionMode.Decompress ]( sourceFilePathName ),
						compressorMap[ System.IO.Compression.CompressionMode.Decompress ]
					)
				},
			};
			RegisterActionMap( typeof( DeflateFile ), actionMap );
		}

		public DeflateFile() : base() {
		}
		#endregion .ctor


		#region static methods
		private static System.String PruneExtension( System.String sourceFileName ) {
			return System.IO.Path.GetFileNameWithoutExtension( sourceFileName );
		}
		private static System.String AddExtension( System.String sourceFilename ) {
			return System.IO.Path.GetFileName( sourceFilename ) + ".gzip";
		}
		#endregion static methods

	}

}
