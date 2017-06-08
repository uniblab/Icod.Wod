using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"renameFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class RenameFile : FileOperationBase {

		#region fields
		private FileDescriptor myDestination;
		#endregion fields


		#region .ctor
		public RenameFile() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement( 
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false 
		)]
		public virtual FileDescriptor Destination {
			get {
				return myDestination;
			}
			set {
				myDestination = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}
			var dest = this.Destination.GetFileHandler();
			var source = this.GetFileHandler();
			if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( order, source as LocalFileHandler, dest as LocalFileHandler );
			} else {
				this.DoWork( order, source, dest );
			}
		}

		private void DoWork( WorkOrder order, FileHandlerBase source, FileHandlerBase dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			} else if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( order, source as LocalFileHandler, dest as LocalFileHandler );
			}

			var filePathName = source.ListFiles().First().File;
			using ( var reader = source.OpenReader( filePathName ) ) {
				dest.Overwrite( reader, source.PathCombine( source.FileDescriptor.ExpandedPath, dest.FileDescriptor.ExpandedName ) );
			}
			source.DeleteFile( filePathName );
		}

		private void DoWork( WorkOrder order, LocalFileHandler source, LocalFileHandler dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}

			var filePathName = source.ListFiles().First().File;
			System.IO.File.Move( filePathName, source.PathCombine( source.FileDescriptor.ExpandedPath, dest.FileDescriptor.ExpandedName ) );
		}
		#endregion methods

	}

}