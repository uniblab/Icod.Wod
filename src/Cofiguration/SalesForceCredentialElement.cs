// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Configuration {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		IncludeInSchema = false,
		Namespace = "http://Icod.Wod"
	)]
	public sealed class SalesForceCredentialElement : System.Configuration.ConfigurationElement, SalesForce.ICredential {

		#region fields
		[System.NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		private System.Uri mySiteUrl;
		#endregion fields


		#region .ctor
		public SalesForceCredentialElement() : base() {
			mySiteUrl = null;
		}
		#endregion .ctor


		#region properties
		[System.Configuration.ConfigurationProperty( "name", IsRequired = true, IsKey = true, DefaultValue = (System.String)null )]
		public System.String Name {
			get {
				return ( this[ "name" ] as System.String ).TrimToNull();
			}
			set {
				this[ "name" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "clientId", IsRequired = true, IsKey = false, DefaultValue = (System.String)null )]
		public System.String ClientId {
			get {
				return ( this[ "clientId" ] as System.String ).TrimToNull();
			}
			set {
				this[ "clientId" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "loginMode", IsRequired = true, IsKey = false, DefaultValue = SalesForce.LoginMode.RefreshToken )]
		public SalesForce.LoginMode LoginMode {
			get {
				return (SalesForce.LoginMode)( this[ "loginMode" ] );
			}
			set {
				this[ "loginMode" ] = value;
			}
		}

		[System.Configuration.ConfigurationProperty( "clientSecret", IsRequired = true, IsKey = false, DefaultValue = (System.String)null )]
		public System.String ClientSecret {
			get {
				return ( this[ "clientSecret" ] as System.String ).TrimToNull();
			}
			set {
				this[ "clientSecret" ] = value.TrimToNull();
			}
		}

		[System.Configuration.ConfigurationProperty( "username", IsRequired = false, IsKey = false, DefaultValue = (System.String)null )]
		public System.String Username {
			get {
				return ( this[ "username" ] as System.String ).TrimToNull();
			}
			set {
				this[ "username" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "password", IsRequired = false, IsKey = false, DefaultValue = (System.String)null )]
		public System.String Password {
			get {
				return ( this[ "password" ] as System.String ).TrimToNull();
			}
			set {
				this[ "password" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "securityToken", IsRequired = false, IsKey = false, DefaultValue = (System.String)null )]
		public System.String SecurityToken {
			get {
				return ( this[ "securityToken" ] as System.String ).TrimToNull();
			}
			set {
				this[ "securityToken" ] = value.TrimToNull();
			}
		}

		[System.Configuration.ConfigurationProperty( "scheme", IsRequired = false, IsKey = false, DefaultValue = "https" )]
		public System.String Scheme {
			get {
				return (System.String)this[ "scheme" ];
			}
			set {
				mySiteUrl = null;
				this[ "scheme" ] = value;
			}
		}
		[System.Configuration.ConfigurationProperty( "host", IsRequired = true, IsKey = false, DefaultValue = (System.String)null )]
		public System.String Host {
			get {
				return (System.String)this[ "host" ];
			}
			set {
				mySiteUrl = null;
				this[ "host" ] = value;
			}
		}
		[System.Configuration.ConfigurationProperty( "port", IsRequired = false, IsKey = false, DefaultValue = "-1" )]
		public System.Int32 Port {
			get {
				return (System.Int32)this[ "port" ];
			}
			set {
				mySiteUrl = null;
				this[ "port" ] = value;
			}
		}
		[System.Configuration.ConfigurationProperty( "path", IsRequired = true, IsKey = false, DefaultValue = (System.String)null )]
		public System.String Path {
			get {
				return (System.String)this[ "path" ];
			}
			set {
				mySiteUrl = null;
				this[ "path" ] = value;
			}
		}
		[System.Xml.Serialization.XmlIgnore]
		public System.Uri SiteUrl {
			get {
				if ( null == mySiteUrl ) {
					var host = this.Host;
					System.Uri probe = System.String.IsNullOrEmpty( host )
						? null
						: new System.UriBuilder( this.Scheme, host, this.Port, this.Path ?? System.String.Empty ).Uri
					;
					System.Threading.Interlocked.CompareExchange<System.Uri>( ref mySiteUrl, probe, null );
				}
				return mySiteUrl;
			}
		}

		[System.Configuration.ConfigurationProperty( "callbackUrl", IsRequired = false, IsKey = false, DefaultValue = (System.String)null )]
		public System.String CallbackUrl {
			get {
				return (System.String)this[ "callbackUrl" ];
			}
			set {
				this[ "callbackUrl" ] = value;
			}
		}
		[System.Configuration.ConfigurationProperty( "refreshToken", IsRequired = false, IsKey = false, DefaultValue = (System.String)null )]
		public System.String RefreshToken {
			get {
				return (System.String)this[ "refreshToken" ];
			}
			set {
				this[ "refreshToken" ] = value;
			}
		}
		#endregion properties

	}

}
