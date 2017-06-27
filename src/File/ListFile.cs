using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"copyFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ListFile : FileOperationBase {

		#region .ctor
		public ListFile() : base() {
		}
		public ListFile( WorkOrder workOrder ) {
		}
		#endregion .ctor


		#region properties
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			throw new NotImplementedException();
		}
		#endregion methods

	}

}