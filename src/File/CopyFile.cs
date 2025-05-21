// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"copyFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class CopyFile : BinaryFileOperationBase, IMove {

		#region fields
		private System.Boolean myMove;

		private static readonly System.Action<System.String, System.String> theMoveFile;
		private static readonly System.Action<System.String, System.String> theCopyFile;
		#endregion fields


		#region .ctor
		static CopyFile() {
			theMoveFile = ( sourceFilePathName, destFilePathName ) => {
				var f = new System.IO.FileInfo( sourceFilePathName );
				_ = f.CopyTo( destFilePathName, true );
				f.Delete();
			};
			theCopyFile = ( sourceFilePathName, destFilePathName ) => new System.IO.FileInfo( sourceFilePathName ).CopyTo( destFilePathName, true );
		}

		public CopyFile() : base() {
			myMove = false;
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
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			if ( ( dest is null ) || ( source is null ) ) {
				throw new System.InvalidOperationException();
			} else if ( ( source is LocalFileHandler ) && ( dest is LocalFileHandler ) ) {
				this.DoWork( source as LocalFileHandler, dest as LocalFileHandler );
				return;
			}
			System.Action<FileHandlerBase, System.String> delFile;
			if ( this.Move ) {
				delFile = ( s, f ) => s.DeleteFile( f );
			} else {
				delFile = ( s, f ) => {
				};
			}
			var files = source.ListFiles();
			var fileEntries = files.Select(
				x => x.File
			);
			foreach ( var file in fileEntries ) {
				using ( var reader = source.OpenReader( file ) ) {
					var dfd = dest.FileDescriptor;
					dest.Overwrite( reader, dfd.GetFilePathName( dest, file ) );
				}
				delFile( source, file );
			}
		}

		private void DoWork( LocalFileHandler source, LocalFileHandler dest ) {
#if DEBUG
			if ( dest is null ) {
				throw new System.ArgumentNullException( nameof( dest ) );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( nameof( source ) );
			}
#endif
			var dfd = dest.FileDescriptor;
			System.String file;
			var files = source.ListFiles();
			var action = ( this.Move )
				? theMoveFile
				: theCopyFile
			;
			foreach ( var fe in files ) {
				file = fe.File;
				action( file, dfd.GetFilePathName( dest, file ) );
			}
		}
		#endregion methods

	}

}
