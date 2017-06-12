using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public sealed class TextFileColumn {

		#region fields
		private System.Int32 myLength;
		private System.String myFormatString;
		#endregion fields


		#region .ctor
		public TextFileColumn() : base() {
			myLength = -1;
			myFormatString = "{0}";
		}
		public TextFileColumn( System.String name ) : this() {
			this.Name = name;
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
		[System.ComponentModel.DefaultValue( "{0}" )]
		public System.String FormatString {
			get {
				return myFormatString;
			}
			set {
				myFormatString = value;
			}
		}
		#endregion properties

	}

}