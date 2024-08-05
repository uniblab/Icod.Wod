// Copyright (C) 2024  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"appendFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class AppendFile : BinaryFileOperationBase, IMove {

		#region fields
		private System.Boolean myMove;
		#endregion fields


		#region .ctor
		public AppendFile() : base() {
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
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var dfd = dest.FileDescriptor;
			var source = this.GetFileHandler( workOrder );
			System.Action<FileHandlerBase, System.String> delFile;
			if ( this.Move ) {
				delFile = ( s, f ) => s.DeleteFile( f );
			} else {
				delFile = ( s, f ) => {
				};
			}
			var files = source.ListFiles().Select(
				x => x.File
			);
			foreach ( var file in files ) {
				using ( var reader = source.OpenReader( file ) ) {
					dest.Append( reader, dfd.GetFilePathName( dest, file ) );
				}
				delFile( source, file );
			}
		}
		#endregion methods

	}

}
