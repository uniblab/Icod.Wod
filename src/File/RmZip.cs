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
		public sealed override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
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
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Update ) ) {
					entries = ( this.MatchEntries( zipArchive.Entries ) ?? new System.IO.Compression.ZipArchiveEntry[ 0 ] ).Reverse();
					foreach ( var e in entries ) {
						e.Delete();
					}
					isEmpty = !zipArchive.Entries.Any();
				}
				if ( isEmpty && deleteIfEmpty ) {
					handler.DeleteFile( file );
				} else {
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, file );
				}
			}
		}
		#endregion methods

	}

}