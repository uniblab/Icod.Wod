namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlInclude( typeof( ChopFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DeleteFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ExecuteFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( MkDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RmDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TouchFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ZipOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryFileOperationBase ) )]
	[System.Xml.Serialization.XmlType(
		"fileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class FileOperationBase : FileBase, IStep {

		#region .ctor
		protected FileOperationBase() : base() {
		}
		protected FileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public IStack<ContextRecord> Context {
			get;
			set;
		}
		#endregion properties


		#region methods
		public virtual void DoWork( WorkOrder workOrder ) {
			this.DoWork( workOrder, Stack<ContextRecord>.Empty );
		}
		public abstract void DoWork( WorkOrder workOrder, IStack<ContextRecord> context );
		#endregion methods

	}

}