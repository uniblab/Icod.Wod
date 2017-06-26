namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlInclude( typeof( AppendFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ChopFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CopyFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DeleteFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( GZipFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( MkDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RenameFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RmDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TouchFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ZipOperationBase ) )]
	[ System.Xml.Serialization.XmlType(
		"fileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class FileOperationBase : FileDescriptor, IStep {

		#region .ctor
		protected FileOperationBase() : base() {
		}
		protected FileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public abstract void DoWork( WorkOrder workOrder );
		#endregion methods

	}

}