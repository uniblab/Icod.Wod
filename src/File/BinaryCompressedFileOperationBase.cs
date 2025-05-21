// Copyright (C) 2025  Timothy J. Bruce

using System.Runtime.CompilerServices;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( GZipFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DeflateFile ) )]
	[System.Xml.Serialization.XmlType(
		"binaryCompressedFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryCompressedFileOperationBase : BinaryFileOperationBase {

		#region fields
		private static readonly System.Collections.Generic.Dictionary<
			System.Type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.IO.Stream, System.IO.Stream>
			>
		> theCompressorMap;
		private static readonly System.Collections.Generic.Dictionary<
			System.Type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.String, System.String>
			>
		> theFileNameMap;
		private static readonly System.Collections.Generic.Dictionary<
			System.Type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Action<FileHandlerBase, System.String, FileHandlerBase>
			>
		> theActionMap;

		private System.Boolean myDelete;
		private System.IO.Compression.CompressionMode myCompressionMode;
		#endregion fields


		#region .ctor
		static BinaryCompressedFileOperationBase() {
			theCompressorMap = new System.Collections.Generic.Dictionary<
				System.Type,
				System.Collections.Generic.Dictionary<
					System.IO.Compression.CompressionMode,
					System.Func<System.IO.Stream, System.IO.Stream>
				>
			>( 2 );
			theFileNameMap = new System.Collections.Generic.Dictionary<
				System.Type,
				System.Collections.Generic.Dictionary<
					System.IO.Compression.CompressionMode,
					System.Func<System.String, System.String>
				>
			>( 2 );
			theActionMap = new System.Collections.Generic.Dictionary<
				System.Type,
				System.Collections.Generic.Dictionary<
					System.IO.Compression.CompressionMode,
					System.Action<FileHandlerBase, System.String, FileHandlerBase>
				>
			>( 2 );
		}

		protected BinaryCompressedFileOperationBase() : base() {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"delete",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Delete {
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
		public System.IO.Compression.CompressionMode CompressionMode {
			get {
				return myCompressionMode;
			}
			set {
				myCompressionMode = value;
			}
		}
		#endregion properties


		#region static methods
		private static void RegisterMap<T>(
			System.Type type,
			System.Collections.Generic.Dictionary<
				System.Type,
				System.Collections.Generic.Dictionary<
					System.IO.Compression.CompressionMode,
					T
				>
			> dict,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				T
			> map
		) {
			dict.Add(
				type,
				map
			);
		}
		protected static void RegisterCompressorMap(
			System.Type type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.IO.Stream, System.IO.Stream>
			> compressorMap
		) {
			RegisterMap( type, theCompressorMap, compressorMap );
		}
		protected static void RegisterFileNameMap(
			System.Type type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Func<System.String, System.String>
			> fileNameMap
		) {
			RegisterMap( type, theFileNameMap, fileNameMap );
		}
		protected static void RegisterActionMap(
			System.Type type,
			System.Collections.Generic.Dictionary<
				System.IO.Compression.CompressionMode,
				System.Action<FileHandlerBase, System.String, FileHandlerBase>
			> actionMap
		) {
			RegisterMap( type, theActionMap, actionMap );
		}

		private static R GetMap<R>( 
			System.Type type, 
			System.IO.Compression.CompressionMode compressionMode,
			System.Collections.Generic.Dictionary<
				System.Type,
				System.Collections.Generic.Dictionary<
					System.IO.Compression.CompressionMode,
					R
				>
			> dict
		) {
			R map;
			try {
				map = dict[ type ][ compressionMode ];
			} catch (System.Exception e ) {
				throw new System.InvalidOperationException(
					System.String.Format( "The specified compressionMode, {0}, is not supported.", compressionMode ),
					e
				);
			}
			return map;
		}
		protected static System.Func<System.IO.Stream, System.IO.Stream> GetCompressorMap( System.Type type, System.IO.Compression.CompressionMode compressionMode ) {
			return GetMap( type, compressionMode, theCompressorMap );
		}
		protected static System.Func<System.String, System.String> GetFileNameMap( System.Type type, System.IO.Compression.CompressionMode compressionMode ) {
			return GetMap( type, compressionMode, theFileNameMap );
		}
		protected static System.Action<FileHandlerBase, System.String, FileHandlerBase> GetActionMap( System.Type type, System.IO.Compression.CompressionMode compressionMode ) {
			return GetMap( type, compressionMode, theActionMap );
		}

		protected static void Decompress(
			Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName,
			Icod.Wod.File.FileHandlerBase dest, System.String destFilePathName,
			System.Func<System.IO.Stream, System.IO.Stream> compressor
		) {
#if DEBUG
			dest = dest ?? throw new System.ArgumentNullException( nameof( dest ) );
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
#endif
			if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( sourceFilePathName ) );
			} else if ( System.String.IsNullOrEmpty( destFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( destFilePathName ) );
			}
			using ( var reader = source.OpenReader( sourceFilePathName ) ) {
				using ( var worker = compressor( reader ) ) {
					dest.Overwrite(
						worker,
						dest.PathCombine( dest.FileDescriptor.ExpandedPath, destFilePathName )
					);
				}
			}
		}
		protected static void Compress(
			Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName,
			Icod.Wod.File.FileHandlerBase dest, System.String destFilePathName,
			System.Func<System.IO.Stream, System.IO.Stream> compressor
		) {
#if DEBUG
			dest = dest ?? throw new System.ArgumentNullException( nameof( dest ) );
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
#endif
			if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( sourceFilePathName ) );
			} else if ( System.String.IsNullOrEmpty( destFilePathName ) ) {
				throw new System.ArgumentNullException( nameof( destFilePathName ) );
			}
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var worker = compressor( buffer ) ) {
					using ( var reader = source.OpenReader( sourceFilePathName ) ) {
						reader.CopyTo( worker );
						worker.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						dest.Overwrite(
							buffer,
							dest.PathCombine( dest.FileDescriptor.ExpandedPath, destFilePathName )
						);
					}
				}
			}
		}
		#endregion static methods

	}

}
