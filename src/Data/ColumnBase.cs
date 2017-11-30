using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( LiteralColumn ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TextFileColumn ) )]
	public abstract class ColumnBase {

		#region fields
		private System.Int32 myLength;
		#endregion fields


		#region .ctor
		protected ColumnBase() : base() {
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
			"nullReplacementText",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String NullReplacementText {
			get;
			set;
		}
		#endregion properties


		#region methods
		public abstract System.String GetColumnText( System.Object value );
		#endregion methods

	}

}