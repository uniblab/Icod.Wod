using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileExport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileExport : DbDescriptor, IStep {

		#region fields
		private FileBase myDestination;
		#endregion fields


		#region .ctor
		public FileExport() : base() {
		}
		public FileExport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement( "destination", Type = typeof( FileBase ), IsNullable = false, Namespace = "http://Icod.Wod" )]
		public FileBase Destination {
			get {
				return myDestination;
			}
			set {
				myDestination = value;
			}
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder order ) {
			this.WorkOrder = order;
			this.Destination.WorkOrder = order;
			this.Destination.WriteRecords( order, this );
		}
		#endregion methods

	}

}