// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fileDescriptor",
		Namespace = "http://Icod.Wod"
	)]
	public class FileDescriptor : IFile {

		#region fields
		private static readonly System.Collections.Generic.Dictionary<System.String, System.Func<WorkOrder, FileDescriptor, FileHandlerBase>> theFileHandlerMap;

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
		static FileDescriptor() {
			theFileHandlerMap = new System.Collections.Generic.Dictionary<System.String, System.Func<WorkOrder, FileDescriptor, FileHandlerBase>>( 6 ) {
				{
					System.String.Empty,
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( LocalFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"file",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( LocalFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"ftp",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( FtpFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"ftps",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( FtpFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"http",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( HttpFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"https",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( HttpFileHandler ), new System.Object[ 2 ] { wo, fd } )
				},
				{
					"sftp",
					( wo, fd ) => (FileHandlerBase)System.Activator.CreateInstance( typeof( SftpFileHandler ), new System.Object[ 2 ] { wo, fd } )
				}
			};
		}

		public FileDescriptor() : base() {
			myUsePassive = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"path",
			Namespace = "http://Icod.Wod"
		)]
#if NETFRAMEWORK
		[System.IO.IODescription( "The directory of interest" )]
#endif
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
				return this.WorkOrder.ExpandPseudoVariables( myPath );
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
#if NETFRAMEWORK
		[System.IO.IODescription( "File name filter pattern" )]
#endif
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
				return this.WorkOrder.ExpandPseudoVariables( myName );
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"regexPattern",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
#if NETFRAMEWORK
		[System.IO.IODescription( "File name or path regular expression pattern" )]
#endif
		public virtual System.String RegexPattern {
			get {
				return myRegexPattern;
			}
			set {
				myRegexPattern = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String ExpandedRegexPattern {
			get {
				return this.WorkOrder.ExpandPseudoVariables( myRegexPattern );
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

		[System.Xml.Serialization.XmlElement(
			ElementName = "sshKeyFile",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		[System.ComponentModel.DefaultValue( null )]
		public SshKeyFileDescriptor SshKeyFile {
			get;
			set;
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
			FileHandlerBase output;

			this.WorkOrder = workOrder;
			var ep = this.ExpandedPath ?? System.String.Empty;

			if ( System.String.IsNullOrEmpty( ep ) ) {
				output = theFileHandlerMap[ ep ]( workOrder, this );
			} else {
				var uri = new System.Uri( ep );
				var scheme = uri.Scheme.ToLower();
				try {
					output = theFileHandlerMap[ scheme ]( workOrder, this );
				} catch ( System.Exception e ) {
					throw new System.InvalidOperationException(
						System.String.Format( "The specified uri scheme, {0}, is not supported.", scheme ),
						e
					);
				}
			}
			return output;
		}

		public virtual System.String GetFileName( System.String alternateName ) {
			var output = this.ExpandedName.TrimToNull();
			alternateName = alternateName.TrimToNull();
			if ( !System.String.IsNullOrEmpty( alternateName ) ) {
				output = output ?? System.IO.Path.GetFileName( alternateName );
			}
			return output;
		}
		public virtual System.String GetFilePathName( FileHandlerBase handler, System.String alternateName ) {
			if ( handler is null ) {
				throw new System.ArgumentNullException( nameof( handler ) );
			}
			return handler.PathCombine( this.ExpandedPath, this.GetFileName( alternateName ) );
		}
		#endregion methods

	}

}
