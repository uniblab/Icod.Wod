// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"deleteFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class DeleteFile : FileOperationBase {

		#region .ctor
		public DeleteFile() : base() {
		}
		public DeleteFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.GetFileHandler( workOrder ).DeleteFile();
		}
		#endregion methods

	}

}
