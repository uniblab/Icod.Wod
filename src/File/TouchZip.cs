// Copyright 2022, Timothy J. Bruce
using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"touchZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class TouchZip : ZipOperationBase {


		#region .ctor
		public TouchZip() : base() {
		}
		public TouchZip( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var handler = this.GetFileHandler( workOrder );
			var file = handler.ListFiles().Select(
				x => x.File
			).FirstOrDefault();
			var zipName = handler.PathCombine( this.ExpandedPath, this.ExpandedName );
			using ( var buffer = new System.IO.MemoryStream() ) {
				if ( null == file ) {
					using ( var zipArchive = this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create ) ) {
						;
					}
					buffer.Flush();
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					handler.Overwrite( buffer, zipName );
				} else {
					handler.Append( buffer, zipName );
				}
			}
		}
		#endregion methods

	}

}
