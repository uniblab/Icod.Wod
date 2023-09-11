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
		"touchZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class TouchZip : ZipOperationBase {


		#region .ctor
		public TouchZip() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			var handler = this.GetFileHandler( workOrder );
			var file = handler.ListFiles().Select(
				x => x.File
			).FirstOrDefault();
			var zipName = handler.PathCombine( this.ExpandedPath!, this.ExpandedName! );
			using ( var buffer = new System.IO.MemoryStream() ) {
				if ( file is null ) {
					using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create ) ) {
						;
					}
					buffer.Flush();
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, zipName );
				} else {
					handler.Append( buffer, zipName );
				}
			}
		}
		#endregion methods

	}

}
