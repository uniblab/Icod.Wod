using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"deleteFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class DeleteFile : FileOperationBase {

		#region .ctor
		public DeleteFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			this.GetFileHandler().DeleteFile();
		}
		#endregion methods

	}

}