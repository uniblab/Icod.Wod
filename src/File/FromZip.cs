// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fromZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FromZip : BinaryZipOperationBase {

		#region .ctor
		public FromZip() : base() {
		}
		#endregion .ctor


		#region method
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var destD = this.Destination;
			destD.WorkOrder = workOrder;
			System.String ePath = destD.ExpandedPath;
			var dest = destD.GetFileHandler( workOrder );

			var handler = this.GetFileHandler( workOrder );
			System.String file;
			System.IO.Stream buffer;
			System.String eDir;
			foreach ( var zipFile in handler.ListFiles().Where(
				x => x.FileType.Equals( FileType.File )
			) ) {
				buffer = new System.IO.MemoryStream();
				file = zipFile.File;
				using ( var a = handler.OpenReader( file ) ) {
					a.CopyTo( buffer );
				}
				_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Read ) ) {
					foreach ( var entry in this.MatchEntries( zipArchive.Entries ) ) {
						eDir = ( this.TruncateEntryName )
							? ePath
							: dest.PathCombine( ePath, System.IO.Path.GetDirectoryName( entry.FullName ) )
						;
						using ( var entryStream = entry.Open() ) {
							dest.Overwrite( entryStream, dest.PathCombine( eDir, entry.Name ) );
						}
					}
				}
			}
		}
	}
	#endregion methods

}
