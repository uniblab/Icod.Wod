// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( Data.DataFileBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( File.FileRedirection ) )]
	public abstract class FileBase : Icod.Wod.File.FileDescriptor {

		#region fields
		public const System.String DefaultCodePage = "windows-1252";
		public const System.Int32 DefaultBufferLength = 16384;
		public const System.String DefaultRecordSeparator = "\r\n";

		private System.String myCodePage;
		private System.Boolean myWriteIfEmpty;
		private System.Int32 myBufferLength;
		private System.String myRecordSeparator;
		#endregion fields


		#region .ctor
		protected FileBase() : base() {
			myCodePage = DefaultCodePage;
			myBufferLength = DefaultBufferLength;
			myWriteIfEmpty = false;
			myRecordSeparator = DefaultRecordSeparator;
		}
		protected FileBase( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = DefaultCodePage;
			myBufferLength = DefaultBufferLength;
			myWriteIfEmpty = false;
			myRecordSeparator = DefaultRecordSeparator;
		}
		#endregion  .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"bufferLength",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultBufferLength )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultCodePage )]
		public System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultRecordSeparator )]
		public virtual System.String RecordSeparator {
			get {
				return myRecordSeparator;
			}
			set {
				myRecordSeparator = value;
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
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}
		#endregion methods

	}

}
