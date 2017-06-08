using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"copyFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class CopyFile : FileOperationBase {

		#region fields
		private FileDescriptor myDestination;
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

		[System.Xml.Serialization.XmlAttribute(
			"move",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public virtual System.Boolean Move {
			get {
				return myMove;
			}
			set {
				myMove = value;
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
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			} else if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( order, source as LocalFileHandler, dest as LocalFileHandler );
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

		private void DoWork( WorkOrder order, LocalFileHandler source, LocalFileHandler dest ) {
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
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