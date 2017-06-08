using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"dbTable",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class DbTable : DbDescriptor {

		#region .ctor
		public DbTable() : base() {
		}
		#endregion .ctor

	}

}