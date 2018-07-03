namespace Icod.Wod {

	[System.Serializable]
	public sealed class ContextRecord {

		public ContextRecord() : base() {
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Data.DataRow Record {
			get;
			set;
		}

	}

}