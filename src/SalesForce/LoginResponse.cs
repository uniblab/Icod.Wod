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
			if ( null == response ) {
				throw new System.ArgumentNullException( "response" );
			};
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
