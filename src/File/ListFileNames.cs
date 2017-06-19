using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"listFileNames",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	internal sealed class ListFileNames : ListBase {

		#region .ctor
		public ListFileNames() : base() {
		}
		public ListFileNames( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}
			this.WorkOrder = order;
			this.Destination.WorkOrder = order;
			var dest = this.Destination.GetFileHandler( order );
			var source = this.GetFileHandler( order );

			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			var files = source.ListFiles().Where(
				x => FileType.File.Equals( x.FileType )
			);
			if ( this.WriteIfEmpty && !files.Any() ) {
				return;
			}
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding() ) ) {
					foreach ( var fe in files ) {
						writer.WriteLine( fe.File );
					}
				}
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				dest.Overwrite( buffer, dest.PathCombine( dest.FileDescriptor.ExpandedPath, dest.FileDescriptor.ExpandedName ) );
			}
		}
		#endregion methods

	}

}