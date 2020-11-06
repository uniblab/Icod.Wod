using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"serial",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Serial : IStep {

		#region .ctor
		public Serial() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "steps"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Email ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Parallel ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Serial ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( File.FileOperationBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileExport ),
			ElementName = "dbFileExport",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileImport ),
			ElementName = "dbFileImport",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.DbCommand ),
			ElementName = "dbCommand",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Rest.RestSelect ),
			ElementName = "sfRestSelect",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.Object[] Steps {
			get;
			set;
		}

		[System.Xml.Serialization.XmlIgnore]
		public WorkOrder WorkOrder {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			foreach ( var step in ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>() ) {
				step.DoWork( workOrder );
			}
		}
		#endregion methods

	}

}