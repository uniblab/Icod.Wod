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
		"list",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class List : FileOrDirectoryListerBase {

		#region .ctor
		public List() : base() {
		}
		public List( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			return source.List();
		}
		public sealed override void DoWork( WorkOrder workOrder ) {
			var source = this.GetFileHandler( workOrder );
			if ( null == source ) {
				throw new System.InvalidOperationException();
			}

			System.Func<FileEntry, System.String> getFileName;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.File )!;
			} else {
				getFileName = x => x.File;
			}

			var list = this.GetEntries( source );
			if ( this.WriteIfEmpty || list.Any() ) {
				var dest = this.Destination;
				dest.WorkOrder = workOrder;
				var dh = dest.GetFileHandler( workOrder );
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength, true ) ) {
						foreach ( var entry in list ) {
							writer.WriteLine( "\"{0}\",\"{1}\"", entry.FileType, getFileName( entry ) );
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dh.Overwrite( buffer, dh.PathCombine( dest.ExpandedPath!, dest.ExpandedName! ) );
				}
			}
		}
	}
	#endregion methods

}
