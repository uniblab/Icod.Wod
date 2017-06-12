using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"rmDir",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class RmDir : FileOperationBase {

		#region .ctor
		public RmDir() : base() {
		}
		public RmDir( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.WorkOrder = order;
			this.GetFileHandler( order ).RmDir();
		}
		#endregion methods

	}

}