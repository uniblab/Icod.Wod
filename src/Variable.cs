using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"variable",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Variable {

		public Variable() : base() {
		}


		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Name {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"value",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Value {
			get;
			set;
		}

	}

}