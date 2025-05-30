// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"listFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ListFile : FileOrDirectoryListerBase {

		#region .ctor
		public ListFile() : base() {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
			return source.ListFiles();
		}
		#endregion methods

	}

}
