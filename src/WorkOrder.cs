using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlRoot(
		ElementName = "WorkOrder",
		Namespace = "http://Icod.Wod",
		IsNullable = false
	)]
	public class WorkOrder {

		#region fields
		private ConnectionStringEntry[] myConnectionStrings;
		private System.Object[] mySteps;
		private System.String myJobName;
		private System.String myEmailTo;
		#endregion fields


		#region .ctor
		public WorkOrder() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"jobName",
			Namespace = "http://Icod.Wod"
		)]
		public System.String JobName {
			get {
				return myJobName;
			}
			set {
				myJobName = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"emailTo",
			Namespace = "http://Icod.Wod"
		)]
		public System.String EmailTo {
			get {
				return myEmailTo;
			}
			set {
				myEmailTo = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "connectionStrings"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"connectionString",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public ConnectionStringEntry[] ConnectionStrings {
			get {
				return myConnectionStrings;
			}
			set {
				myConnectionStrings = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "steps"
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
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileExport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileImport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.DbCommand ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public System.Object[] Steps {
			get {
				return mySteps;
			}
			set {
				mySteps = value;
			}
		}
		#endregion properties


		#region methods
		public void Run() {
			System.Int32 i = 1;
			IStep step = null;
			try {
				var steps = ( this.Steps ?? new System.Object[ 0 ] ).OfType<IStep>().ToArray();
				foreach ( var s in steps ) {
					step = s;
					s.DoWork( this );
					i++;
				}
			} catch ( System.Exception e ) {
				throw new Icod.Wod.Exception( e.Message, e, i, this, step );
			}
		}
		#endregion methods

	}

}