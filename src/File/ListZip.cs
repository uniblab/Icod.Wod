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
		"listZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ListZip : BinaryZipOperationBase {

		#region .ctor
		public ListZip() : base() {
		}
		public ListZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			var handler = this.GetFileHandler( workOrder );
			if ( null == handler ) {
				throw new System.InvalidOperationException();
			}

			var destD = this.Destination;
			destD.WorkOrder = workOrder;
			var dest = destD.GetFileHandler( workOrder );
			if ( null == dest ) {
				throw new System.InvalidOperationException();
			}

			System.Func<System.IO.Compression.ZipArchiveEntry, System.String> getFileName;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.Name )!;
			} else {
				getFileName = x => x.FullName;
			}

			System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> list;
			using ( var reader = handler.OpenReader( handler.PathCombine( this.ExpandedPath!, this.ExpandedName! ) ) ) {
				using ( var zip = this.GetZipArchive( reader, System.IO.Compression.ZipArchiveMode.Read ) ) {
					list = this.MatchEntries( zip.Entries );
				}
			}
			if ( this.WriteIfEmpty || list.Any() ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength, true ) ) {
						foreach ( var item in list.Select(
							x => getFileName( x )
						) ) {
							writer.WriteLine( item );
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dest.Overwrite( buffer, dest.PathCombine( destD.ExpandedPath!, destD.ExpandedName! ) );
				}
			}
		}
		#endregion methods

	}

}
