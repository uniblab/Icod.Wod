using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public class TextFileColumn {

		#region .ctor
		public TextFileColumn() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Name {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"length",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( -1 )]
		public System.Int32 Length {
			get;
			set;
		}
		#endregion properties

	}

}