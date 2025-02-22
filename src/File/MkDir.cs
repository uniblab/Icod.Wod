// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"mkDir",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class MkDir : FileOperationBase {

		#region .ctor
		public MkDir() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.GetFileHandler( workOrder ).MkDir();
		}
		#endregion methods

	}

}
