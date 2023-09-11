// Icod.Wod.dll is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

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
		private System.Boolean myDelete;
		private System.IO.Compression.CompressionMode myCompressionMode;
		#endregion fields


		#region .ctor
		protected BinaryCompressedFileOperationBase() : base() {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		protected BinaryCompressedFileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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

		[System.Xml.Serialization.XmlIgnore]
		protected System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream>? Activator {
			get;
			set;
		}
		#endregion properties


		#region methods
		public override void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.Destination.WorkOrder = workOrder;
			System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase, System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream>> action;
			switch ( this.CompressionMode ) {
				case System.IO.Compression.CompressionMode.Decompress:
					action = this.Decompress;
					break;
				case System.IO.Compression.CompressionMode.Compress:
					action = this.Compress;
					break;
				default:
					throw new System.InvalidOperationException();
			}

			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			System.String file;
			var files = source.ListFiles();
			foreach ( var fe in files ) {
				file = fe.File;
				action( source, file, dest, this.Activator! );
			}
			if ( this.Delete ) {
				foreach ( var fe in files ) {
					source.DeleteFile( fe.File );
				}
			}
		}
		protected void Decompress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest, System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> activator ) {
			var dfd = dest.FileDescriptor;
			using ( var reader = source.OpenReader( sourceFilePathName ) ) {
				using ( var worker = activator( reader, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						worker.CopyTo( buffer );
						buffer.Flush();
						_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath!, fn ) );
					}
				}
			}
		}
		protected void Compress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest, System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> activator ) {
			var dfd = dest.FileDescriptor;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var worker = activator( buffer, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var reader = source.OpenReader( sourceFilePathName ) ) {
						reader.CopyTo( worker );
						worker.Flush();
						_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath!, fn ) );
					}
				}
			}
		}
		protected System.String GetDestinatonFileName( System.String source ) {
			System.String fn;
			switch ( this.CompressionMode ) {
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
		#endregion methods

	}

}
