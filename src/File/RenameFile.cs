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
		"renameFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class RenameFile : BinaryFileOperationBase {

		#region .ctor
		public RenameFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.Destination!.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( ( source as LocalFileHandler )!, ( dest as LocalFileHandler )! );
			} else {
				this.DoWork( workOrder, source, dest );
			}
		}

		private void DoWork( WorkOrder workOrder, FileHandlerBase source, FileHandlerBase dest ) {
			if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( ( source as LocalFileHandler )!, ( dest as LocalFileHandler )! );
			}

			var filePathName = source.ListFiles().First().File;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var reader = source.OpenReader( filePathName ) ) {
					reader.CopyTo( buffer );
				}
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				dest.Overwrite( buffer, source.PathCombine( source.FileDescriptor.ExpandedPath!, dest.FileDescriptor.ExpandedName! ) );
			}
			source.DeleteFile( filePathName );
		}

		private void DoWork( LocalFileHandler source, LocalFileHandler dest ) {
			var file = source.ListFiles().FirstOrDefault();
			if ( null == file ) {
				return;
			}
			var filePathName = file.File;
			System.IO.File.Move( filePathName, source.PathCombine( source.FileDescriptor.ExpandedPath!, dest.FileDescriptor.ExpandedName! ) );
		}
		#endregion methods

	}

}
