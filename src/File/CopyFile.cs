using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"copyFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class CopyFile : BinaryFileOperationBase {

		#region fields
		private System.Boolean myMove;

		private static readonly System.Action<System.String, System.String> theMoveFile;
		private static readonly System.Action<System.String, System.String> theCopyFile;
		#endregion fields


		#region .ctor
		static CopyFile() {
			theMoveFile = ( sourceFilePathName, destFilePathName ) => {
				var f = new System.IO.FileInfo( sourceFilePathName );
				f.CopyTo( destFilePathName, true );
				f.Delete();
			};
			theCopyFile = ( sourceFilePathName, destFilePathName ) => new System.IO.FileInfo( sourceFilePathName ).CopyTo( destFilePathName, true );
		}

		public CopyFile() : base() {
		}
		public CopyFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"move",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Move {
			get {
				return myMove;
			}
			set {
				myMove = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			if ( ( null == dest ) || ( null == source ) ) {
				throw new System.InvalidOperationException();
			} else if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( source as LocalFileHandler, dest as LocalFileHandler );
				return;
			}
			var files = source.ListFiles();
			System.String file;
			foreach ( var fe in files ) {
				file = fe.File;
				using ( var reader = source.OpenReader( file ) ) {
					dest.Overwrite( reader, dest.PathCombine( dest.FileDescriptor.ExpandedPath, System.IO.Path.GetFileName( file ) ) );
				}
			}
			if ( this.Move ) {
				foreach ( var fe in source.ListFiles() ) {
					source.DeleteFile( fe.File );
				}
			}
		}

		private void DoWork( LocalFileHandler source, LocalFileHandler dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			System.String file;
			var files = source.ListFiles();
			System.Action<System.String, System.String> action = ( this.Move )
				? theMoveFile
				: theCopyFile
			;
			foreach ( var fe in files ) {
				file = fe.File;
				action( file, dest.PathCombine( dest.FileDescriptor.ExpandedPath, System.IO.Path.GetFileName( file ) ) );
			}
		}
		#endregion methods

	}

}