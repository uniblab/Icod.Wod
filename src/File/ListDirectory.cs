// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"listDirectory",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ListDirectory : FileOrDirectoryListerBase {

		#region .ctor
		public ListDirectory() : base() {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			if ( source is null ) {
				throw new System.ArgumentNullException( nameof( source ) );
			}
			return source.ListDirectories();
		}
		#endregion methods

	}

}
