using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"appendFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class AppendFile : FileOperationBase {

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
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}

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
		public sealed override void DoWork( WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}
			this.WorkOrder = order;
			this.Destination.WorkOrder = order;
			var dest = this.Destination.GetFileHandler( order );
			var dfd = dest.FileDescriptor;
			var source = this.GetFileHandler( order );
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