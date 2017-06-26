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
			System.Func<FileEntry, System.String> getFileName = x => this.TruncateEntryName
				? System.IO.Path.GetFileName( x.File )
				: x.File
			;
			System.String fileName;
			System.IO.Compression.ZipArchiveEntry entry;
			using ( System.IO.Stream buffer = new System.IO.MemoryStream() ) {
				using ( var zipArchive = new System.IO.Compression.ZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create, false, this.GetEncoding() ) ) {
					foreach ( var file in source.ListFiles().Where(
						x => x.FileType.Equals( FileType.File )
					) ) {
						using ( var reader = source.OpenReader( file.File ) ) {
							fileName = getFileName( file );
							entry = zipArchive.CreateEntry( fileName );
							using ( var writer = entry.Open() ) {
								reader.CopyTo( writer );
							}
						}
					}
				}
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				handler.Overwrite( buffer, handler.PathCombine( this.ExpandedPath, this.ExpandedName ) );
			}

			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}