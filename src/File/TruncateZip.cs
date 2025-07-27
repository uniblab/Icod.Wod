// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"truncateZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class TruncateZip : ZipOperationBase {


		#region .ctor
		public TruncateZip() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var handler = this.GetFileHandler( workOrder );
			var file = handler.ListFiles().Select(
				x => x.File
			).FirstOrDefault();
			if ( file is null ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					this.GetZipArchive( buffer, System.IO.Compression.ZipArchiveMode.Create ).Dispose();
					buffer.Flush();
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					var zipName = handler.PathCombine( this.ExpandedPath, this.ExpandedName );
					handler.Overwrite( buffer, zipName );
				}
			} else {
				handler.TruncateFile();
			}
		}
		#endregion methods

	}

}
