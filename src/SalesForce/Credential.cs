using System.Linq;

namespace Icod.Wod.SalesForce {

	[System.Serializable]
	public sealed class Credential : SalesForce.ICredential {

		#region .ctor
		public Credential() : base() {
		}
		#endregion .ctor


		#region properties
		public System.String ClientId {
			get;
			set;
		}
		public System.String ClientSecret {
			get;
			set;
		}
		public System.String Username {
			get;
			set;
		}
		public System.String Password {
			get;
			set;
		}
		public System.String SecurityToken {
			get;
			set;
		}
		public System.Uri SiteUrl {
			get;
			set;
		}
		#endregion properties

	}

}