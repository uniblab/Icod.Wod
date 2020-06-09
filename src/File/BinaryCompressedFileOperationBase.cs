using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( GZipFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DeflateFile ) )]
	[System.Xml.Serialization.XmlType(
		"binaryCompressedFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryCompressedFileOperationBase : BinaryFileOperationBase {

		#region fields
		private System.Boolean myDelete;
		private System.IO.Compression.CompressionMode myCompressionMode;
		#endregion fields


		#region .ctor
		protected BinaryCompressedFileOperationBase() : base() {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		protected BinaryCompressedFileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"delete",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Delete {
			get {
				return myDelete;
			}
			set {
				myDelete = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"compressionMode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.IO.Compression.CompressionMode.Decompress )]
		public System.IO.Compression.CompressionMode CompressionMode {
			get {
				return myCompressionMode;
			}
			set {
				myCompressionMode = value;
			}
		}
		#endregion properties

	}

}