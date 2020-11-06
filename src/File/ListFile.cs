using System.Linq;

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
		public ListFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			return source.ListFiles();
		}
		#endregion methods

	}

}
