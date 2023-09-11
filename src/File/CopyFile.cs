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
	[System.Xml.Serialization.XmlType(
		"copyFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class CopyFile : BinaryFileOperationBase, IMove {

		#region fields
		private System.Boolean myMove;

		private static readonly System.Action<System.String, System.String> theMoveFile;
		private static readonly System.Action<System.String, System.String> theCopyFile;
		#endregion fields


		#region .ctor
		static CopyFile() {
			theMoveFile = ( sourceFilePathName, destFilePathName ) => {
				var f = new System.IO.FileInfo( sourceFilePathName );
				_ = f.CopyTo( destFilePathName, true );
				f.Delete();
			};
			theCopyFile = ( sourceFilePathName, destFilePathName ) => new System.IO.FileInfo( sourceFilePathName ).CopyTo( destFilePathName, true );
		}

		public CopyFile() : base() {
			myMove = false;
		}
		public CopyFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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
			this.Destination!.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			if ( ( dest is null ) || ( source is null ) ) {
				throw new System.InvalidOperationException();
			} else if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( (source as LocalFileHandler)!, (dest as LocalFileHandler)! );
				return;
			}
			System.Action<FileHandlerBase, System.String> delFile;
			if ( this.Move ) {
				delFile = ( s, f ) => s.DeleteFile( f );
			} else {
				delFile = ( s, f ) => {
				};
			}
			var files = source.ListFiles();
			var fileEntries = files.Select(
				x => x.File
			);
			foreach ( var file in fileEntries ) {
				using ( var reader = source.OpenReader( file ) ) {
					var dfd = dest.FileDescriptor;
					dest.Overwrite( reader, dfd.GetFilePathName( dest, file ) );
				}
				delFile( source, file );
			}
		}

		private void DoWork( LocalFileHandler source, LocalFileHandler dest ) {
			var dfd = dest.FileDescriptor;
			System.String file;
			var files = source.ListFiles();
			var action = ( this.Move )
				? theMoveFile
				: theCopyFile
			;
			foreach ( var fe in files ) {
				file = fe.File;
				action( file, dfd.GetFilePathName( dest, file ) );
			}
		}
		#endregion methods

	}

}
