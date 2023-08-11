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

using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"mkZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class MkZip : ZipOperationBase, IMove {

		#region fields
		private System.Boolean myMove;

		private static readonly System.Action<FileHandlerBase, System.String> theMoveFile;
		private static readonly System.Action<FileHandlerBase, System.String> theCopyFile;
		#endregion fields


		#region .ctor
		static MkZip() {
			theMoveFile = ( handler, sourceFilePathName ) => handler.DeleteFile( sourceFilePathName );
			theCopyFile = ( handler, sourceFilePathName ) => { ; };
		}

		public MkZip() : base() {
			myMove = false;
		}
		public MkZip( WorkOrder workOrder ) : base( workOrder ) {
			myMove = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"move",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Move {
			get {
				return myMove;
			}
			set {
				myMove = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sources = ( this.Source ?? new FileDescriptor[ 0 ] ).Select(
				x => {
					x.WorkOrder = workOrder;
					return x;
				}
			);
			FileHandlerBase source;
			System.String sep;
			var handler = this.GetFileHandler( workOrder );
			System.String fileName;
			System.IO.Compression.ZipArchiveEntry entry;
			var writeIfEmpty = this.WriteIfEmpty;
			var isEmpty = true;
			System.Action<FileHandlerBase, System.String> fileAct = null;
			if ( this.Move ) {
				fileAct = theMoveFile;
			} else {
				fileAct = theCopyFile;
			}
			using ( System.IO.Stream buffer = new System.IO.MemoryStream() ) {
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create ) ) {
					foreach ( var sourceD in sources ?? new FileDescriptor[ 0 ] ) {
						sep = sourceD.ExpandedPath;
						source = sourceD.GetFileHandler( workOrder );
						foreach ( var file in source.ListFiles().Where(
							x => x.FileType.Equals( FileType.File )
						) ) {
							using ( var reader = source.OpenReader( file.File ) ) {
								fileName = this.ProcessFileName( file, sep );
								if ( !System.String.IsNullOrEmpty( fileName ) ) {
									entry = zipArchive.CreateEntry( fileName, System.IO.Compression.CompressionLevel.Optimal );
									using ( var writer = entry.Open() ) {
										reader.CopyTo( writer );
										isEmpty = false;
									}
								}
							}
							fileAct( source, file.File );
						}
					}
				}
				if ( !isEmpty || writeIfEmpty ) {
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, handler.PathCombine( this.ExpandedPath, this.ExpandedName ) );
				}
			}
		}
		#endregion methods

	}

}
