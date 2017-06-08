using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"touchFile",
		Namespace = "http://Icod.Wod"
	)]
	public class TouchFile : FileOperationBase {

		#region .ctor
		public TouchFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.GetFileHandler().TouchFile();
		}
		#endregion methods

	}

}