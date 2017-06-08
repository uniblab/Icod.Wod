namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlType(
		"fileDescriptor",
		Namespace = "http://Icod.Wod"
	)]
	public class FileDescriptor : IFile {

		#region fields
		private System.String myPath;
		[System.NonSerialized]
		private System.String myExpandedPath;
		private System.String myName;
		[System.NonSerialized]
		private System.String myExpandedName;
		private System.String myRegexPattern;
		private System.IO.SearchOption mySearchOption;
		private System.Boolean myRecurse;
		private System.String myUsername;
		private System.String myPassword;
		private System.Boolean myUsePassive;
		#endregion fields


		#region .ctor
		public FileDescriptor() : base() {
			myUsePassive = true;
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
				myExpandedPath = this.ExpandVariables( value );
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String ExpandedPath {
			get {
				return myExpandedPath;
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
				myExpandedName = this.ExpandVariables( value );
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String ExpandedName {
			get {
				return myExpandedName;
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
			"recurse",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public virtual System.Boolean Recurse {
			get {
				return myRecurse;
			}
			set {
				myRecurse = value;
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
		#endregion properties


		#region methods
		public FileHandlerBase GetFileHandler() {
			FileHandlerBase output = null;

			var uri = new System.Uri( this.ExpandedPath );
			switch ( uri.Scheme.ToLower() ) {
				case "file" :
					output = new LocalFileHandler( this );
					break;
				case "ftp" :
				case "ftps" :
					output = new FtpFileHandler( this );
					break;
				case "http":
				case "https":
					output = new FtpFileHandler( this );
					break;
				case "sftp":
					output = new SftpFileHandler( this );
					break;
				default :
					throw new System.NotSupportedException();
			}

			return output;
		}

		protected System.String ExpandVariables( System.String path ) {
			path = path.TrimToNull();
			if ( System.String.IsNullOrEmpty( path ) ) {
				return path;
			}
			var now = System.DateTime.Now;
			if ( path.Contains( "%wod:DateTime-" ) ) {
				var yy = now.ToString( "yy" );
				while ( path.Contains( "%wod:DateTime-yy%" ) ) {
					path = path.Replace( "%wod:DateTime-yy%", yy );
				}
				var yyyy = now.ToString( "yyyy" );
				while ( path.Contains( "%wod:DateTime-yyyy%" ) ) {
					path = path.Replace( "%wod:DateTime-yyyy%", yyyy );
				}
				var MM = now.ToString( "MM" );
				while ( path.Contains( "%wod:DateTime-MM%" ) ) {
					path = path.Replace( "%wod:DateTime-MM%", MM );
				}
				var dd = now.ToString( "dd" );
				while ( path.Contains( "%wod:DateTime-dd%" ) ) {
					path = path.Replace( "%wod:DateTime-dd%", dd );
				}
				var yyyyMMdd = now.ToString( "yyyyMMdd" );
				while ( path.Contains( "%wod:DateTime-yyyyMMdd%" ) ) {
					path = path.Replace( "%wod:DateTime-yyyyMMdd%", yyyyMMdd );
				}
			}
			return path;
		}
		#endregion methods

	}

}