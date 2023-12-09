// Copyright 2023, Timothy J. Bruce

namespace Icod.Wod {

	[System.Serializable]
	public sealed class ConnectionStringEntry {

		#region fields
		private System.String myProviderName;
		#endregion fields


		#region .ctor
		public ConnectionStringEntry() : base() {
			myProviderName = "System.Data.SqlClient";
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
			"providerName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "System.Data.SqlClient" )]
		public System.String ProviderName {
			get {
				return myProviderName;
			}
			set {
				myProviderName = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"connectionString",
			Namespace = "http://Icod.Wod"
		)]
		public System.String ConnectionString {
			get;
			set;
		}
		#endregion properties

	}

}
