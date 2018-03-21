using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileExport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileExport : DbFileBase {

		#region fields
		private DataFileBase myDestination;
		#endregion fields


		#region .ctor
		public FileExport() : base() {
		}
		public FileExport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement( "destination", Type = typeof( DataFileBase ), IsNullable = false, Namespace = "http://Icod.Wod" )]
		public DataFileBase Destination {
			get {
				return myDestination;
			}
			set {
				myDestination = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			this.Destination.WriteRecords( workOrder, this );
		}
		#endregion methods

	}

}