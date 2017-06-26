using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fromZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ToZip : ZipOperationBase {

		#region .ctor
		public ToZip() : base() {
		}
		public ToZip( WorkOrder workOrder ) : base( workOrder ) {
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
			var sourceD = this.Source;
			sourceD.WorkOrder = workOrder;
			var source = sourceD.GetFileHandler( workOrder );
			var destD = this.Destination;
			destD.WorkOrder = workOrder;
			var dest = destD.GetFileHandler( workOrder );
			var handler = this.GetFileHandler( workOrder );
			System.String file;
			System.IO.Stream buffer;
			System.String eDir;

			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}