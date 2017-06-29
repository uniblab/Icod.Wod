using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"executeFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ExecuteFile : FileOperationBase {

		#region fields
		#endregion fields


		#region .ctor
		public ExecuteFile() : base() {
		}
		public ExecuteFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"stdOut",
			Namespace = "http://Icod.Wod"
		)]
		public FileRedirection StdOut {
			get;
			set;
		}

		[System.Xml.Serialization.XmlElement(
			"stdErr",
			Namespace = "http://Icod.Wod"
		)]
		public FileRedirection StdErr {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			throw new NotImplementedException();
		}
		#endregion methods

	}

}