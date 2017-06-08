using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public class TextFileColumn {

		#region fields
		private System.Int32 myLength;
		#endregion fields


		#region .ctor
		public TextFileColumn() : base() {
			myLength = -1;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
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
			get {
				return myLength;
			}
			set {
				myLength = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"formatString",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String FormatString {
			get;
			set;
		}
		#endregion properties

	}

}