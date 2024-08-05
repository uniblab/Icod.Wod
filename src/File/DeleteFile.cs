// Copyright (C) 2024  Timothy J. Bruce

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
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.GetFileHandler( workOrder ).DeleteFile();
		}
		#endregion methods

	}

}
