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
			var sources = this.Source.Select(
				x => {
					x.WorkOrder = workOrder;
					return x;
				}
			);
			FileHandlerBase source;
			System.String sep;
			var handler = this.GetFileHandler( workOrder );
			System.Func<FileEntry, System.String, System.String> getFileName = null;
			if ( this.TruncateEntryName ) {
				getFileName = ( x, y ) => System.IO.Path.GetFileName( x.File );
			} else {
				getFileName = ( x, y ) => x.File.Replace( y, System.String.Empty );
			}
			System.String fileName;
			System.IO.Compression.ZipArchiveEntry entry;
			var writeIfEmpty = this.WriteIfEmpty;
			var isEmpty = true;
			using ( System.IO.Stream buffer = new System.IO.MemoryStream() ) {
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create ) ) {
					foreach ( var sourceD in sources ?? new FileDescriptor[ 0 ] ) {
						sep = sourceD.ExpandedPath;
						source = sourceD.GetFileHandler( workOrder );
						foreach ( var file in source.ListFiles().Where(
							x => x.FileType.Equals( FileType.File )
						) ) {
							using ( var reader = source.OpenReader( file.File ) ) {
								fileName = getFileName( file, sep );
								fileName = fileName.Replace( '\\', '/' );
								while ( fileName.StartsWith( "/", StringComparison.OrdinalIgnoreCase ) ) {
									fileName = fileName.Substring( 1 );
								}
								entry = zipArchive.CreateEntry( fileName, System.IO.Compression.CompressionLevel.Optimal );
								using ( var writer = entry.Open() ) {
									reader.CopyTo( writer );
									isEmpty = false;
								}
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