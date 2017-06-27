using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"mkZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class MkZip : ZipOperationBase {

		#region .ctor
		public MkZip() : base() {
		}
		public MkZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceD = this.Source;
			sourceD.WorkOrder = workOrder;
			var source = sourceD.GetFileHandler( workOrder );
			var handler = this.GetFileHandler( workOrder );
			System.Func<FileEntry, System.String> getFileName = null;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.File );
			} else {
				getFileName = x => x.File;
			}
			System.String fileName;
			System.IO.Compression.ZipArchiveEntry entry;
			var writeIfEmpty = this.WriteIfEmpty;
			var isEmpty = true;
			using ( System.IO.Stream buffer = new System.IO.MemoryStream() ) {
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Update ) ) {
					foreach ( var file in source.ListFiles().Where(
						x => x.FileType.Equals( FileType.File )
					) ) {
						using ( var reader = source.OpenReader( file.File ) ) {
							fileName = getFileName( file );
							entry = zipArchive.CreateEntry( fileName );
							using ( var writer = entry.Open() ) {
								reader.CopyTo( writer );
								isEmpty = false;
							}
						}
					}
				}
				if ( !isEmpty || writeIfEmpty ) {
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, handler.PathCombine( this.ExpandedPath, this.ExpandedName ) );
				}
			}
		}
		#endregion methods

	}

}