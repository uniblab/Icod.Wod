using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileImport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileImport : DbDescriptor, IStep {

		#region fields
		private FileBase mySource;
		#endregion fields


		#region .ctor
		public FileImport() : base() {
		}
		public FileImport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement( "source", Type = typeof( FileBase ), IsNullable = false, Namespace = "http://Icod.Wod" )]
		public FileBase Source {
			get {
				return mySource;
			}
			set {
				mySource = value;
			}
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder order ) {
			this.WriteRecords( order, this.Source );
		}
		#endregion methods

	}

}