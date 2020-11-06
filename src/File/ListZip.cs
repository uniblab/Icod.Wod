using System;
using System.Linq;

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
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
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

			System.Func<System.IO.Compression.ZipArchiveEntry, System.String> getFileName = null;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.Name );
			} else {
				getFileName = x => x.FullName;
			}

			System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> list = null;
			using ( var reader = handler.OpenReader( handler.PathCombine( this.ExpandedPath, this.ExpandedName ) ) ) {
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
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dest.Overwrite( buffer, dest.PathCombine( destD.ExpandedPath, destD.ExpandedName ) );
				}
			}
		}
		#endregion methods

	}

}
