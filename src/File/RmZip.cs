// Copyright (C) 2025  Timothy J. Bruce
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
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
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
					entries = ( this.MatchEntries( zipArchive.Entries ) ?? System.Array.Empty<System.IO.Compression.ZipArchiveEntry>() ).Reverse();
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
