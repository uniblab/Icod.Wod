namespace Icod.Wod.Data {

	[System.Xml.Serialization.XmlInclude( typeof( Command ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileExport ) )]
	[System.Xml.Serialization.XmlType(
		"dbOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class DbOperationBase : DbDescriptorBase, Icod.Wod.IStep {

		#region .ctor
		protected DbOperationBase() : base() {
		}
		protected DbOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public abstract void DoWork( WorkOrder workOrder );
		#endregion methods

	}

}