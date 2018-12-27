using System.Linq;

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
		public ListDirectory( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			return source.ListDirectories();
		}
		#endregion methods

	}

}