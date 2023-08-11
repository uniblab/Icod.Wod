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

using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"gzipFile",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class GZipFile : BinaryCompressedFileOperationBase {

		#region .ctor
		public GZipFile() : base() {
		}
		public GZipFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			System.Action<Icod.Wod.File.FileHandlerBase, System.String, Icod.Wod.File.FileHandlerBase> action;
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
		private void Decompress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest ) {
#if DEBUG
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
#endif
			if (System.String.IsNullOrEmpty(sourceFilePathName ) ) {
				throw new System.ArgumentNullException( "sourceFilePathName" );
			}
			var dfd = dest.FileDescriptor;
			using ( var reader = source.OpenReader( sourceFilePathName ) ) {
				using ( var worker = new System.IO.Compression.GZipStream( reader, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						worker.CopyTo( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private void Compress( Icod.Wod.File.FileHandlerBase source, System.String sourceFilePathName, Icod.Wod.File.FileHandlerBase dest ) {
#if DEBUG
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
#endif
			if ( System.String.IsNullOrEmpty( sourceFilePathName ) ) {
				throw new System.ArgumentNullException( "sourceFilePathName" );
			}
			var dfd = dest.FileDescriptor;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var worker = new System.IO.Compression.GZipStream( buffer, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var reader = source.OpenReader( sourceFilePathName ) ) {
						reader.CopyTo( worker );
						worker.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						var fn = this.GetDestinatonFileName( sourceFilePathName );
						dest.Overwrite( buffer, dest.PathCombine( dfd.ExpandedPath, fn ) );
					}
				}
			}
		}
		private System.String GetDestinatonFileName( System.String source ) {
#if DEBUG
			if ( System.String.IsNullOrEmpty( source ) ) {
				throw new System.ArgumentNullException( "source" );
			}
#endif
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
