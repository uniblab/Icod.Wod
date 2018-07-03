using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileImport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileImport : DbFileBase {

		#region fields
		private DataFileBase mySource;
		#endregion fields


		#region .ctor
		public FileImport() : base() {
		}
		public FileImport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement( "source", Type = typeof( DataFileBase ), IsNullable = false, Namespace = "http://Icod.Wod" )]
		public DataFileBase Source {
			get {
				return mySource;
			}
			set {
				mySource = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.WriteRecords( workOrder, this.Source );
		}
		#endregion methods

	}

}