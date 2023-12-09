// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"renameFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class RenameFile : BinaryFileOperationBase {

		#region .ctor
		public RenameFile() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( source as LocalFileHandler, dest as LocalFileHandler );
			} else {
				this.DoWork( workOrder, source, dest );
			}
		}

		private void DoWork( WorkOrder workOrder, FileHandlerBase source, FileHandlerBase dest ) {
#if DEBUG
			if ( dest is null ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
#endif
			if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( source as LocalFileHandler, dest as LocalFileHandler );
			}

			var filePathName = source.ListFiles().First().File;
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var reader = source.OpenReader( filePathName ) ) {
					reader.CopyTo( buffer );
				}
				buffer.Seek( 0, System.IO.SeekOrigin.Begin );
				dest.Overwrite( buffer, source.PathCombine( source.FileDescriptor.ExpandedPath, dest.FileDescriptor.ExpandedName ) );
			}
			source.DeleteFile( filePathName );
		}

		private void DoWork( LocalFileHandler source, LocalFileHandler dest ) {
#if DEBUG
			if ( dest is null ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			}
#endif

			var file = source.ListFiles().FirstOrDefault();
			if ( file is null ) {
				return;
			}
			var filePathName = file.File;
			System.IO.File.Move( filePathName, source.PathCombine( source.FileDescriptor.ExpandedPath, dest.FileDescriptor.ExpandedName ) );
		}
		#endregion methods

	}

}
