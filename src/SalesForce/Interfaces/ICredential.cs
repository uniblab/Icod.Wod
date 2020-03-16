namespace Icod.Wod.SalesForce {

	public interface ICredential {

		System.String Name {
			get;
		}
		System.String ClientId {
			get;
			set;
		}
		SalesForce.LoginMode LoginMode {
			get;
			set;
		}
		System.String ClientSecret {
			get;
			set;
		}
		System.String Username {
			get;
			set;
		}
		System.String Password {
			get;
			set;
		}
		System.String SecurityToken {
			get;
			set;
		}
		System.String Scheme {
			get;
			set;
		}
		System.String Host {
			get;
			set;
		}
		System.Int32 Port {
			get;
			set;
		}
		System.String Path {
			get;
			set;
		}
		System.Uri SiteUrl {
			get;
		}

		System.String CallbackUrl {
			get;
			set;
		}
		System.String RefreshToken {
			get;
			set;
		}

		System.Int32 MaxDegreeOfParallelism {
			get;
		}

	}

}