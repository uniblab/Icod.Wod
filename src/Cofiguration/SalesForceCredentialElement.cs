using System.Linq;

namespace Icod.Wod.Configuration {

	[System.Serializable]
	public sealed class SalesForceCredentialElement : System.Configuration.ConfigurationElement, SalesForce.ICredential {

		#region .ctor
		public SalesForceCredentialElement() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Configuration.ConfigurationProperty( "clientId", IsRequired = true, IsKey = true )]
		public System.String ClientId {
			get {
				return ( this[ "clientId" ] as System.String ).TrimToNull();
			}
			set {
				this[ "clientId" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "clientSecret", IsRequired = true, IsKey = false )]
		public System.String ClientSecret {
			get {
				return ( this[ "clientSecret" ] as System.String ).TrimToNull();
			}
			set {
				this[ "clientSecret" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "username", IsRequired = true, IsKey = false )]
		public System.String Username {
			get {
				return ( this[ "username" ] as System.String ).TrimToNull();
			}
			set {
				this[ "username" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "password", IsRequired = true, IsKey = false )]
		public System.String Password {
			get {
				return ( this[ "password" ] as System.String ).TrimToNull();
			}
			set {
				this[ "password" ] = value.TrimToNull();
			}
		}
		[System.Configuration.ConfigurationProperty( "securityToken", IsRequired = true, IsKey = false )]
		public System.String SecurityToken {
			get {
				return ( this[ "securityToken" ] as System.String ).TrimToNull();
			}
			set {
				this[ "securityToken" ] = value.TrimToNull();
			}
		}

		[System.Configuration.ConfigurationProperty( "siteUrl", IsRequired = true, IsKey = false )]
		public System.Uri SiteUrl {
			get {
				return (System.Uri)this[ "siteUrl" ];
			}
			set {
				this[ "siteUrl" ] = value;
			}
		}
		#endregion properties

	}

}