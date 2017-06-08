using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public class ColumnMap {

		#region fields
		private System.Boolean mySkip;
		private System.String myFromName;
		private System.String myToName;
		#endregion fields


		#region .ctor
		public ColumnMap() : base() {
			mySkip = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"skip",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Skip {
			get {
				return mySkip;
			}
			set {
				mySkip = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"from",
			Namespace = "http://Icod.Wod"
		)]
		public System.String FromName {
			get {
				return myFromName;
			}
			set {
				myFromName = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"to",
			Namespace = "http://Icod.Wod"
		)]
		public System.String ToName {
			get {
				return myToName;
			}
			set {
				myToName = value;
			}
		}
		#endregion properties

	}

}