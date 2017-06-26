using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fromZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FromZip : ZipOperationBase {

		#region .ctor
		public FromZip() : base() {
		}
		public FromZip( WorkOrder workOrder ) : base( workOrder ) {
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


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}