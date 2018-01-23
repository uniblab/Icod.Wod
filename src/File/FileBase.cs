using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( Data.DataFileBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( File.FileRedirection ) )]
	public abstract class FileBase : Icod.Wod.File.FileDescriptor {

		#region fields
		private System.String myCodePage;
		private System.Boolean myWriteIfEmpty;
		private System.Int32 myBufferLength;
		#endregion fields


		#region .ctor
		protected FileBase() : base() {
			myCodePage = "windows-1252";
			myBufferLength = 16384;
			myWriteIfEmpty = false;
		}
		protected FileBase( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myBufferLength = 16384;
			myWriteIfEmpty = false;
		}
		#endregion  .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "windows-1252" )]
		public System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"writeIfEmpty",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean WriteIfEmpty {
			get {
				return myWriteIfEmpty;
			}
			set {
				myWriteIfEmpty = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"bufferLength",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 16384 )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}
		#endregion methods

	}

}