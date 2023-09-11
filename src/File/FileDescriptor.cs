// Icod.Wod.dll is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

namespace Icod.Wod.File {

	[System.Serializable]
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
		public virtual System.String? ExpandedPath {
			get {
				return this.WorkOrder.ExpandPseudoVariables( myPath );
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
		public virtual System.String? ExpandedName {
			get {
				return this.WorkOrder.ExpandPseudoVariables( myName );
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
		[System.Xml.Serialization.XmlIgnore]
		public virtual System.String? ExpandedRegexPattern {
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
			var ep = this.ExpandedPath;
			if ( System.String.IsNullOrEmpty( ep ) ) {
				output = new LocalFileHandler( workOrder, this );
			} else {
				var uri = new System.Uri( ep );
				switch ( uri.Scheme.ToLower() ) {
					case "ftp":
					case "ftps":
						output = new FtpFileHandler( workOrder, this );
						break;
					case "http":
					case "https":
						output = new HttpFileHandler( workOrder, this );
						break;
					case "sftp":
						output = new SftpFileHandler( workOrder, this );
						break;
					case "file":
					default:
						output = new LocalFileHandler( workOrder, this );
						break;
				}
			}
			return output;
		}

		public virtual System.String? GetFileName( System.String alternateName ) {
			var output = this.ExpandedName.TrimToNull();
			var aName = alternateName.TrimToNull();
			if ( !System.String.IsNullOrEmpty( aName ) ) {
				output ??= System.IO.Path.GetFileName( aName );
			}
			return output;
		}
		public virtual System.String GetFilePathName( FileHandlerBase handler, System.String alternateName ) {
			if ( handler is null ) {
				throw new System.ArgumentNullException( nameof( handler ) );
			}
			return handler.PathCombine( this.ExpandedPath, this.GetFileName( alternateName )! );
		}
		#endregion methods

	}

}
