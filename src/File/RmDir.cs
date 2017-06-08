using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"rmDir",
		Namespace = "http://Icod.Wod"
	)]
	public class RmDir : FileOperationBase {

		#region .ctor
		public RmDir() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.GetFileHandler().RmDir();
		}
		#endregion methods

	}

}