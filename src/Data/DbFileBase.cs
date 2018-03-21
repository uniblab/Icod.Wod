﻿using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( FileExport ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	public abstract class DbFileBase : DbOperationBase, Icod.Wod.IStep {

		#region .ctor
		protected DbFileBase() : base() {
		}
		protected DbFileBase( Icod.Wod.WorkOrder workOrder ) : this() {
		}
		#endregion .ctor

	}

}