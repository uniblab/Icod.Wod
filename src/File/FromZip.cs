using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fromZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FromZip : ZipOperationBase {

		#region .ctor
		public FromZip() : base() {
		}
		public FromZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.Source;
			var destD = this.Destination;
			destD.WorkOrder = workOrder;
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
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Read ) ) {
					foreach ( var entry in this.ListEntries( zipArchive, source ) ) {
						using ( var entryStream = entry.Open() ) {
							eDir = System.IO.Path.GetDirectoryName( entry.FullName );
							if ( System.String.IsNullOrEmpty( eDir ) ) {
								eDir = dest.PathCombine( destD.ExpandedPath, eDir );
							} else {
								eDir = destD.ExpandedPath;
							}
							dest.Overwrite( entryStream, dest.PathCombine( eDir, entry.Name ) );
						}
					}
				}
			}
		}
	}
	#endregion methods

}