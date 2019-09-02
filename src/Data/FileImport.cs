using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileImport",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class FileImport : DbIODescriptorBase, Icod.Wod.IStep {

		#region .ctor
		public FileImport() : base() {
		}
		public FileImport( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"source",
			Type = typeof( DataFileBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod" )
		]
		public DataFileBase Source {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			if ( System.String.IsNullOrEmpty( this.CommandText ) ) {
				this.WriteRecords( workOrder, this.Source );
			} else {
				this.ExecuteCommand( workOrder, this.Source );
			}
		}
		#endregion methods

	}

}