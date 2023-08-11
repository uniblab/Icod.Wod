// Icod.Wod is the Work on Demand framework.
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
		"rmZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class RmZip : ZipOperationBase {

		#region .ctor
		public RmZip() : base() {
		}
		public RmZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var handler = this.GetFileHandler( workOrder );
			System.String file;
			System.IO.Stream buffer;
			System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> entries;
			var deleteIfEmpty = !this.WriteIfEmpty;
			var isEmpty = true;
			foreach ( var zipFile in handler.ListFiles().Where(
				x => x.FileType.Equals( FileType.File )
			) ) {
				buffer = new System.IO.MemoryStream();
				file = zipFile.File;
				using ( var a = handler.OpenReader( file ) ) {
					a.CopyTo( buffer );
				}
				_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Update ) ) {
					// it is faster to remove files from last-to-first, because files in a Zip archive are stored one after another, much like a Tar archive.
					entries = ( this.MatchEntries( zipArchive.Entries ) ?? new System.IO.Compression.ZipArchiveEntry[ 0 ] ).Reverse();
					foreach ( var e in entries ) {
						e.Delete();
					}
					isEmpty = !zipArchive.Entries.Any();
				}
				if ( isEmpty && deleteIfEmpty ) {
					handler.DeleteFile( file );
				} else {
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, file );
				}
			}
		}
		#endregion methods

	}

}
