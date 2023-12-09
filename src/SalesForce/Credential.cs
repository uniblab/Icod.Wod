// Copyright 2023, Timothy J. Bruce
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
		public System.String Name {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"clientId",
			Namespace = "http://Icod.Wod"
		)]
		public System.String ClientId {
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
		public System.String ClientSecret {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"username",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Username {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"password",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Password {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"securityToken",
			Namespace = "http://Icod.Wod"
		)]
		public System.String SecurityToken {
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
		public System.Uri SiteUrl {
			get {
				if ( mySiteUrl is null) {
					var host = this.Host;
					System.Uri probe = System.String.IsNullOrEmpty( host )
						? null
						: new System.UriBuilder( this.Scheme, host, this.Port, this.Path ?? System.String.Empty ).Uri
					;
					_ = System.Threading.Interlocked.CompareExchange<System.Uri>( ref mySiteUrl, probe, null );
				}
				return mySiteUrl;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"callbackUrl",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String CallbackUrl {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"refreshToken",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String RefreshToken {
			get;
			set;
		}
		#endregion properties


		#region static methods
		public static ICredential GetCredential( System.String name, WorkOrder workOrder ) {
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			ICredential here = ( workOrder.SFCredentials ?? new ICredential[ 0 ] ).FirstOrDefault(
				x => x.Name.Equals( name, System.StringComparison.Ordinal )
			);
			if ( here is object ) {
				return here;
			}
			var section = (Configuration.SalesForceCredentialSection)Configuration.SalesForceCredentialSection.GetSection() ?? throw new System.Configuration.ConfigurationErrorsException( "The icod.wod.sfCredentials is not defined." );
			var collection = section.Credentials;
			if ( ( collection is null ) || ( collection.Count < 1 ) ) {
				throw new System.Configuration.ConfigurationErrorsException( "No such configuration element is defined." );
			}
			return collection[ name ];
		}
		#endregion static methods
	}

}
