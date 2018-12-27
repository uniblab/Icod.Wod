namespace Icod.Wod.Data {

	[System.Serializable]
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