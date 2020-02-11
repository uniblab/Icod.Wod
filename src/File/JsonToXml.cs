using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"jsonToXml",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class JsonToXml : BinaryFileOperationBase {

		#region .ctor
		public JsonToXml() : base() {
		}
		public JsonToXml( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );

			var files = source.ListFiles();
			var fileEntries = files.Select(
				x => x.File
			);
			foreach ( var file in fileEntries ) {
				using ( var reader = source.OpenReader( file ) ) {
					using ( var json = new System.IO.StreamReader( reader, this.GetEncoding(), true, this.BufferLength, true ) ) {
						var doc = (System.Xml.XmlDocument)Newtonsoft.Json.JsonConvert.DeserializeXmlNode( json.ReadToEnd() );
#if DEBUG
						doc.PreserveWhitespace = true;
#else
						doc.PreserveWhitespace = false;
#endif
						using ( var buffer = new System.IO.MemoryStream() ) {
							doc.Save( buffer );
							buffer.Flush();
							buffer.Seek( 0, System.IO.SeekOrigin.Begin );
							dest.Overwrite( buffer, dest.FileDescriptor.GetFilePathName( dest, file ) );
						}
					}
				}
			}
		}
		#endregion methods

	}

}