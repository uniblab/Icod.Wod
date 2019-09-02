using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	//[System.Xml.Serialization.XmlType(
	//	"foreach",
	//	Namespace = "http://Icod.Wod",
	//	IncludeInSchema = true
	//)]
	internal abstract class Foreach : DbOperationBase {

		#region .ctor
		public Foreach() : base() {
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
			typeof( Data.DbOperationBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public System.Object[] Steps {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder ) {
			using ( var cnxn = this.CreateConnection( workOrder ) ) {
				if ( System.Data.ConnectionState.Open != cnxn.State ) {
					cnxn.Open();
				}
				// need to work out the datatableadapter logic here, iterate foreach row, passing this.Context.Push( row ) as the context.
			}
		}
		#endregion methods

	}

}