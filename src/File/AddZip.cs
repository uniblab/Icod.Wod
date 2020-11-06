using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"addZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class AddZip : ZipOperationBase {

		#region .ctor
		public AddZip() : base() {
		}
		public AddZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sources = ( this.Source ?? new FileDescriptor[ 0 ] ).Select(
				x => {
					x.WorkOrder = workOrder;
					return x;
				}
			);

			FileHandlerBase source;
			System.String sep;
			var handler = this.GetFileHandler( workOrder );
			System.String fileName;
			System.IO.Compression.ZipArchiveEntry entry;
			var writeIfEmpty = this.WriteIfEmpty;
			using ( System.IO.Stream buffer = new System.IO.MemoryStream() ) {
				using ( var reader = handler.OpenReader( handler.PathCombine( this.ExpandedPath, this.ExpandedName ) ) ) {
					reader.CopyTo( buffer );
				}
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Update ) ) {
					foreach ( var sourceD in sources ?? new FileDescriptor[ 0 ] ) {
						sep = sourceD.ExpandedPath;
						source = sourceD.GetFileHandler( workOrder );
						foreach ( var file in source.ListFiles().Where(
							x => x.FileType.Equals( FileType.File )
						) ) {
							using ( var reader = source.OpenReader( file.File ) ) {
								fileName = this.ProcessFileName( file, sep );
								if ( !System.String.IsNullOrEmpty( fileName ) ) {
									entry = zipArchive.Entries.FirstOrDefault(
										x => x.FullName.Equals( fileName, StringComparison.OrdinalIgnoreCase )
									) ?? zipArchive.CreateEntry( fileName, System.IO.Compression.CompressionLevel.Optimal );
									using ( var writer = entry.Open() ) {
										reader.CopyTo( writer );
									}
								}
							}
						}
					}
				}
			}
		}
		#endregion methods

	}

}
