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

using System.Linq;

namespace Icod.Wod.SalesForce {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		TypeName = "sfCredential",
		IncludeInSchema = true, 
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Credential : SalesForce.ICredential {

		#region fields
		public const System.Int32 DefaultMaxDegreeOfParallelism = 4;

		private System.String myScheme;
		private System.Int32 myPort;
		private LoginMode myLoginMode;
		[System.NonSerialized]
		private System.Uri mySiteUrl;
		#endregion fields


		#region .ctor
		public Credential() : base() {
			myScheme = "https";
			myPort = -1;
			myLoginMode = LoginMode.RefreshToken;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? Name {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"clientId",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? ClientId {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"loginMode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( LoginMode.RefreshToken )]
		public LoginMode LoginMode {
			get {
				return myLoginMode;
			}
			set {
				myLoginMode = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"clientSecret",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? ClientSecret {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"username",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? Username {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"password",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? Password {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"securityToken",
			Namespace = "http://Icod.Wod"
		)]
		public System.String? SecurityToken {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"scheme",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "https" )]
		public System.String Scheme {
			get {
				return myScheme;
			}
			set {
				myScheme = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"host",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Host {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"port",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( -1 )]
		public System.Int32 Port {
			get {
				return myPort;
			}
			set {
				myPort = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"path",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Path {
			get;
			set;
		}
		public System.Uri? SiteUrl {
			get {
				if ( null == mySiteUrl ) {
					var host = this.Host;
					System.Uri? probe = System.String.IsNullOrEmpty( host )
						? null
						: new System.UriBuilder( this.Scheme, host, this.Port, this.Path ?? System.String.Empty ).Uri
					;
					_ = System.Threading.Interlocked.CompareExchange<System.Uri?>( ref mySiteUrl, probe, null );
				}
				return mySiteUrl;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"callbackUrl",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String CallbackUrl {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"refreshToken",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String RefreshToken {
			get;
			set;
		}
		#endregion properties


		#region static methods
		public static ICredential GetCredential( System.String name, WorkOrder workOrder ) {
			if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			ICredential here = ( workOrder.SFCredentials ?? new ICredential[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( name, System.StringComparison.Ordinal )
			);
			if ( null != here ) {
				return here;
			}
			var section = (Configuration.SalesForceCredentialSection)Configuration.SalesForceCredentialSection.GetSection();
			if ( null == section ) {
				throw new System.Configuration.ConfigurationErrorsException( "The icod.wod.sfCredentials is not defined." );
			}
			var collection = section.Credentials;
			if ( ( null == collection ) || ( collection.Count < 1 ) ) {
				throw new System.Configuration.ConfigurationErrorsException( "No such configuration element is defined." );
			}
			return collection[ name ];
		}
		#endregion static methods
	}

}
