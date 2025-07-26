// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"truncateFile",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class TruncateFile : FileOperationBase {

		#region .ctor
		public TruncateFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			this.GetFileHandler( workOrder ).TruncateFile();
		}
		#endregion methods
	}

}