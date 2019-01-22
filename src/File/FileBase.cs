using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( Data.DataFileBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( File.FileRedirection ) )]
	public abstract class FileBase : Icod.Wod.File.FileDescriptor {

		#region fields
		public const System.String DefaultRecordSeparator = "\r\n";
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;

		private System.String myCodePage;
		private System.Boolean myWriteIfEmpty;
		private System.Int32 myBufferLength;
		private System.String myRecordSeparator;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		#endregion fields


		#region .ctor
		static FileBase() {
			theEmptyEolWriter = x => { };
		}

		protected FileBase() : base() {
			myCodePage = "windows-1252";
			myBufferLength = 16384;
			myWriteIfEmpty = false;
			myRecordSeparator = DefaultRecordSeparator;
			myEolWriter = theEmptyEolWriter;
		}
		protected FileBase( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myBufferLength = 16384;
			myWriteIfEmpty = false;
			myRecordSeparator = DefaultRecordSeparator;
			myEolWriter = theEmptyEolWriter;
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

		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultRecordSeparator )]
		public System.String RecordSeparator {
			get {
				return myRecordSeparator;
			}
			set {
				myRecordSeparator = value;
				myEolWriter = System.String.IsNullOrEmpty( value )
					? theEmptyEolWriter
					: x => x.Write( myRecordSeparator )
				;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		internal System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? theEmptyEolWriter;
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