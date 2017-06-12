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
		public FileHandlerBase Handler {
			get;
			set;
		}
		#endregion properties

	}

}