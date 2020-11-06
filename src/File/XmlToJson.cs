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
			this.ChangeFileExtension = true;
		}
		public XmlToJson( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			this.ChangeFileExtension = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"changeFileExtension",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean ChangeFileExtension {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );

			System.Func<System.String, System.String> correctedFileName = null;
			if ( this.ChangeFileExtension ) {
				correctedFileName = x => System.IO.Path.GetFileNameWithoutExtension( x ) + ".json";
			} else {
				correctedFileName = x => x;
			}
			var files = source.ListFiles();
			var fileEntries = files.Select(
				x => x.File
			);
			foreach ( var file in fileEntries ) {
				using ( var reader = source.OpenReader( file ) ) {
					var doc = new System.Xml.XmlDocument();
					doc.Load( reader );
					Newtonsoft.Json.Formatting formatting;
#if DEBUG
					formatting = Newtonsoft.Json.Formatting.Indented;
#else
					formatting = Newtonsoft.Json.Formatting.None;
#endif
					var json = Newtonsoft.Json.JsonConvert.SerializeXmlNode( doc, formatting );
					using ( var buffer = new System.IO.MemoryStream() ) {
						doc.Save( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						dest.Overwrite( buffer, dest.FileDescriptor.GetFilePathName( dest, correctedFileName( file ) ) );
					}
				}
			}
		}
		#endregion methods

	}

}
