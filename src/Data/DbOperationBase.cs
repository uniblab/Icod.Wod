// Copyright 2022, Timothy J. Bruce
namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DbCommand ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileExport ) )]
	[System.Xml.Serialization.XmlType(
		"dbOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class DbOperationBase : DbDescriptorBase {

		#region .ctor
		protected DbOperationBase() : base() {
		}
		protected DbOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor

	}

}
