using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"appendFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class AppendFile : BinaryFileOperationBase {

		#region fields
		private System.Boolean myMove;
		#endregion fields


		#region .ctor
		public AppendFile() : base() {
			myMove = false;
		}
		public AppendFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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
		public sealed override async void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var dfd = dest.FileDescriptor;
			var source = this.GetFileHandler( workOrder );
			System.String file = source.ListFiles().First().File;
			using ( var reader = source.OpenReader( file ) ) {
				dest.Append( reader, dest.PathCombine( dfd.ExpandedPath, dfd.ExpandedName ) );
			}
			if ( this.Move ) {
				source.DeleteFile( file );
			}
		}
		#endregion methods

	}

}