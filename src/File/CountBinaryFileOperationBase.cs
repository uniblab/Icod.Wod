// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( HeadFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TailFile ) )]
	[System.Xml.Serialization.XmlType(
		"countBinaryFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class CountBinaryFileOperationBase : BinaryFileOperationBase {

		#region fields
		private System.Int32 myCount;
		#endregion fields


		#region .ctor
		protected CountBinaryFileOperationBase() : base() {
			myCount = 0;
		}
		protected CountBinaryFileOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			myCount = 0;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"count",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Count {
			get {
				return myCount;
			}
			set {
				myCount = value;
			}
		}
		#endregion properties


		#region methods
		protected abstract IQueue<System.String> ReadPositiveCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding );
		protected abstract IQueue<System.String> ReadNegativeCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding );
		#endregion methods

	}

}
