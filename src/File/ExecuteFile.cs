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
		private System.String myStdErrCodePage;
		private System.String myStdOutCodePage;
		#endregion fields

	}

}