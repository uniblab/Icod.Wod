namespace Icod.Wod.SalesForce {

	public interface ICredential {

		System.String ClientId {
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
		System.Uri SiteUrl {
			get;
			set;
		}

	}

}