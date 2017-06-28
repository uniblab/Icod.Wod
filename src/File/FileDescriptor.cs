namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlType(
		"fileDescriptor",
		Namespace = "http://Icod.Wod"
	)]
	public class FileDescriptor : IFile {

		#region fields
		private System.String myPath;
		private System.String myName;
		private System.String myRegexPattern;
		private System.IO.SearchOption mySearchOption;
		private System.String myUsername;
		private System.String myPassword;
		private System.Boolean myUsePassive;
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		public FileDescriptor() : base() {
			myUsePassive = true;
		}
		public FileDescriptor( Icod.Wod.WorkOrder workOrder ) : this() {
			myWorkOrder = workOrder;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"path",
			Namespace = "http://Icod.Wod"
		)]
		public virtual System.String Path {
			get {
				return myPath;
			}
			set {
				myPath = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String ExpandedPath {
			get {
				return this.WorkOrder.ExpandVariables( myPath );
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		public virtual System.String Name {
			get {
				return myName;
			}
			set {
				myName = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String ExpandedName {
			get {
				return this.WorkOrder.ExpandVariables( myName );
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"regexPattern",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String RegexPattern {
			get {
				return myRegexPattern;
			}
			set {
				myRegexPattern = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"searchOption",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.IO.SearchOption.TopDirectoryOnly )]
		public virtual System.IO.SearchOption SearchOption {
			get {
				return mySearchOption;
			}
			set {
				mySearchOption = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"username",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String Username {
			get {
				return myUsername;
			}
			set {
				myUsername = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"password",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String Password {
			get {
				return myPassword;
			}
			set {
				myPassword = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"usePassive",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public virtual System.Boolean UsePassive {
			get {
				return myUsePassive;
			}
			set {
				myUsePassive = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public Icod.Wod.WorkOrder WorkOrder {
			get {
				return myWorkOrder;
			}
			set {
				myWorkOrder = value;
			}
		}
		#endregion properties


		#region methods
		public FileHandlerBase GetFileHandler( Icod.Wod.WorkOrder workOrder ) {
			FileHandlerBase output = null;

			this.WorkOrder = workOrder;
			var ep = this.ExpandedPath;
			if ( System.String.IsNullOrEmpty( ep ) ) {
				output = new LocalFileHandler( workOrder, this );
			} else {
				var uri = new System.Uri( ep );
				switch ( uri.Scheme.ToLower() ) {
					case "file":
						output = new LocalFileHandler( workOrder, this );
						break;
					case "ftp":
					case "ftps":
						output = new FtpFileHandler( workOrder, this );
						break;
					case "http":
					case "https":
						output = new FtpFileHandler( workOrder, this );
						break;
					case "sftp":
						output = new SftpFileHandler( workOrder, this );
						break;
					default:
						throw new System.NotSupportedException();
				}
			}
			return output;
		}
		#endregion methods

	}

}