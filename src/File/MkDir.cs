using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"mkDir",
		Namespace = "http://Icod.Wod"
	)]
	public class MkDir : FileOperationBase {

		#region .ctor
		public MkDir() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.GetFileHandler().MkDir();
		}
		#endregion methods

	}

}