using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"existsFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ExistsFile : FileOperationBase {

		#region .ctor
		public ExistsFile() : base() {
		}
		public ExistsFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			throw new NotImplementedException();
		}
		#endregion methods

	}

}