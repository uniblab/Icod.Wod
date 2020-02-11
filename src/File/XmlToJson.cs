using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"xmlToJson",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class XmlToJson : BinaryFileOperationBase {

		#region .ctor
		public XmlToJson() : base() {
		}
		public XmlToJson( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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
					var doc = new System.Xml.XmlDocument();
					doc.Load( reader );
					using ( var buffer = new System.IO.MemoryStream() ) {
						Newtonsoft.Json.Formatting formatting;
#if DEBUG
						formatting = Newtonsoft.Json.Formatting.Indented;
#else
						formatting = Newtonsoft.Json.Formatting.None;
#endif
						var json = Newtonsoft.Json.JsonConvert.SerializeXmlNode( doc, formatting );
						doc.Save( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						dest.Overwrite( buffer, dest.FileDescriptor.GetFilePathName( dest, file ) );
					}
				}
			}
		}
		#endregion methods

	}

}