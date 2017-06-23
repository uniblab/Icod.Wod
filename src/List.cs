using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"list",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class List : IStep {

		#region .ctor
		public List() : base() {
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
			typeof( File.FileOperationBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileExport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileImport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.DbCommand ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public System.Object[] Steps {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			foreach ( var step in ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>() ) {
				step.DoWork( workOrder );
			}
		}
		#endregion methods

	}

}