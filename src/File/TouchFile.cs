// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"touchFile",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class TouchFile : FileOperationBase {

		#region .ctor
		public TouchFile() : base() {
		}
		public TouchFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.GetFileHandler( workOrder ).TouchFile();
		}
		#endregion methods

	}

}
