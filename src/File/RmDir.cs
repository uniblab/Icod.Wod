using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"rmDir",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class RmDir : FileOperationBase {

		#region fields
		private System.Boolean myRecurse;
		#endregion fields


		#region .ctor
		public RmDir() : base() {
			myRecurse = false;
		}
		public RmDir( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myRecurse = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"recurse",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Recurse {
			get {
				return myRecurse;
			}
			set {
				myRecurse = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var fh = this.GetFileHandler( workOrder );
			foreach ( var fe in fh.ListDirectories() ) {
				fh.RmDir( fe.File, this.Recurse );
			}
		}
		#endregion methods

	}

}