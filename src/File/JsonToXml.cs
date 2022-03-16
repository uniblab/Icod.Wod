// Copyright 2022, Timothy J. Bruce
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
			this.ChangeFileExtension = true;
		}
		public JsonToXml( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			this.ChangeFileExtension = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"rootElementName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String RootElementName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"elementName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String ElementName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"encodeSpecialCharacters",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false  )]
		public System.Boolean EncodeSpecialCharacters {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"writeArrayAttribute",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean WriteArrayAttribute {
			get;
			set;
		}

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
				correctedFileName = x => System.IO.Path.GetFileNameWithoutExtension( x ) + ".xml";
			} else {
				correctedFileName = x => x;
			}
			var files = source.ListFiles();
			var fileEntries = files.Select(
				x => x.File
			);
			foreach ( var file in fileEntries ) {
				using ( var reader = source.OpenReader( file ) ) {
					using ( var json = new System.IO.StreamReader( reader, this.GetEncoding(), true, this.BufferLength, true ) ) {
						var jsonString = System.String.IsNullOrEmpty( this.ElementName )
							? json.ReadToEnd()
							: "{\"" + this.ElementName + "\":" + json.ReadToEnd() + "}"
						;
						var doc = (System.Xml.XmlDocument)Newtonsoft.Json.JsonConvert.DeserializeXmlNode( jsonString, this.RootElementName, this.WriteArrayAttribute, this.EncodeSpecialCharacters );
#if DEBUG
						doc.PreserveWhitespace = true;
#else
						doc.PreserveWhitespace = false;
#endif
						using ( var buffer = new System.IO.MemoryStream() ) {
							doc.Save( buffer );
							buffer.Flush();
							buffer.Seek( 0, System.IO.SeekOrigin.Begin );
							dest.Overwrite( buffer, dest.FileDescriptor.GetFilePathName( dest, correctedFileName( file ) ) );
						}
					}
				}
			}
		}
		#endregion methods

	}

}
