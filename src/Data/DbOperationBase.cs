// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DbCommand ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileExport ) )]
	[System.Xml.Serialization.XmlType(
		"dbOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class DbOperationBase : DbDescriptorBase {

		#region .ctor
		protected DbOperationBase() : base() {
		}
		#endregion .ctor

	}

}
