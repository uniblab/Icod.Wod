using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"appendFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class AppendFile : FileOperationBase {

		#region fields
		private FileDescriptor myDestination;
		private System.Boolean myMove;
		#endregion fields


		#region .ctor
		public AppendFile()
			: base() {
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
			var dfd = dest.FileDescriptor;
			var source = this.GetFileHandler();
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