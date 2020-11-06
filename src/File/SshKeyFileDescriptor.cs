namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sshKeyFileDescriptor",
		Namespace = "http://Icod.Wod"
	)]
	public class SshKeyFileDescriptor : FileDescriptor {

		#region fields
		private System.String myKeyFilePassword;
		#endregion fields


		#region .ctor
		public SshKeyFileDescriptor() : base() {
		}
		public SshKeyFileDescriptor( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"keyFilePassword",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String KeyFilePassword {
			get {
				return myKeyFilePassword;
			}
			set {
				myKeyFilePassword = value;
			}
		}
		#endregion properties

	}

}
