// Copyright (C) 2025  Timothy J. Bruce
using System.Net;

namespace Icod.Wod.SalesForce {

	[System.Serializable]
	public sealed class LoginResponse {

		#region fields
		[System.NonSerialized]
		public static readonly System.DateTime EpochStart;
		#endregion fields


		#region .ctor
		static LoginResponse() {
			EpochStart = new System.DateTime( 1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc );
		}

		public LoginResponse() : base() {
		}
		public LoginResponse( dynamic response ) : this() {
			response = response ?? throw new System.ArgumentNullException( nameof( response ) );
			this.AccessToken = (System.String)response.access_token;
			this.InstanceUrl = (System.String)response.instance_url;
			this.Id = (System.String)response.id;
			this.Issued = (System.Int64)response.issued_at;
			this.Signature = response.signature;
		}
		#endregion .ctor


		#region properties
		public System.String AccessToken {
			get;
			set;
		}
		public System.String InstanceUrl {
			get;
			set;
		}
		public System.String Id {
			get;
			set;
		}
		public System.Nullable<System.Int64> Issued {
			get;
			set;
		}
		public System.String Signature {
			get;
			set;
		}
		#endregion properties

	}

}
