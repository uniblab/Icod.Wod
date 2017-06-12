using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"mkDir",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class MkDir : FileOperationBase {

		#region .ctor
		public MkDir() : base() {
		}
		public MkDir( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.WorkOrder = order;
			this.GetFileHandler( order ).MkDir();
		}
		#endregion methods

	}

}