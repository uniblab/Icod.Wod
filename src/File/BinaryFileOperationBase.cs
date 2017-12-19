namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlInclude( typeof( AppendFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CopyFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( GZipFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileOrDirectoryLister ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RenameFile ) )]
	[System.Xml.Serialization.XmlType(
		"binaryFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryFileOperationBase : FileOperationBase {

		#region .ctor
		protected BinaryFileOperationBase() : base() {
		}
		protected BinaryFileOperationBase( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}
		#endregion properties

	}

}