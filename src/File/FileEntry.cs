// Copyright 2020, Timothy J. Bruce
namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FileEntry {

		#region .ctor
		public FileEntry() : base() {
		}
		#endregion .ctor


		#region propeties
		public System.String File {
			get;
			set;
		}
		public FileType FileType {
			get;
			set;
		}
		[System.Xml.Serialization.XmlIgnore]
		public FileHandlerBase Handler {
			get;
			set;
		}
		#endregion properties

	}

}
