// Icod.Wod is the Work on Demand framework.
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
	public sealed class Login {

		#region fields
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		public Login() : base() {
		}
		public Login( WorkOrder workOrder ) : this() {
			myWorkOrder = workOrder;
		}
		#endregion .ctor


		#region properties
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
		public LoginResponse GetLoginResponse( System.String clientId ) {
			if ( System.String.IsNullOrEmpty( clientId ) ) {
				throw new System.ArgumentNullException( "clientId" );
			}
			var credential = Credential.GetCredential( clientId, this.WorkOrder );
			return this.GetLoginResponse( credential, System.Text.Encoding.UTF8 );
		}
		public LoginResponse GetLoginResponse( SalesForce.ICredential credential, System.Text.Encoding encoding ) {
			if ( null == credential ) {
				throw new System.ArgumentNullException( "credential" );
			} else if (
				( LoginMode.RefreshToken == credential.LoginMode )
				&& ( System.String.IsNullOrEmpty( credential.RefreshToken ) || System.String.IsNullOrEmpty( credential.CallbackUrl ) )
			) {
				throw new System.InvalidOperationException( "The specified credential is attempting RefreshToken authentication but does not have a refresh token or callback url configured." );
			} else if (
				( LoginMode.Password == credential.LoginMode )
				&& (
					System.String.IsNullOrEmpty( credential.Username )
					|| ( System.String.IsNullOrEmpty( credential.Password ) && System.String.IsNullOrEmpty( credential.SecurityToken ) )
				)
			) {
				throw new System.InvalidOperationException( "The specified credential is attempting Password authentication but does not have a username or password configured." );
			}

			return this.BuildLogin( credential, encoding );
		}
		private LoginResponse BuildLogin( SalesForce.ICredential credential, System.Text.Encoding encoding ) {
			if ( null == credential ) {
				throw new System.ArgumentNullException( "credential" );
			}

			var headers = new System.Collections.Generic.Dictionary<System.String, System.String>();
			headers.Add( "Content-type", "application/x-www-form-urlencoded; charset=" + encoding.WebName );
#if DEBUG
			headers.Add( "Accept-Encoding", "identity" );
#else
			headers.Add( "Accept-Encoding", "gzip, deflate, identity" );
#endif
			var body = this.BuildBody( credential );
			return this.BuildLogin( credential.SiteUrl, headers, body );
		}
		private System.String BuildBody( SalesForce.ICredential credential ) {
			if ( null == credential ) {
				throw new System.ArgumentNullException( "credential" );
			}

			var clientId = credential.ClientId;
			var parameters = new System.Text.StringBuilder();
			var encoding = System.Text.Encoding.UTF8;
			parameters = parameters.Append( "client_id=" );
			parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( clientId, encoding ) );
			switch ( credential.LoginMode ) {
				case LoginMode.Password:
					var clientSecret = credential.ClientSecret;
					var password = ( credential.Password ?? System.String.Empty ) + ( credential.SecurityToken ?? System.String.Empty );
					if ( System.String.IsNullOrEmpty( password ) ) {
						throw new System.InvalidOperationException( "The specified credential is attempting Password authentication but does not have a password configured." );
					}
					parameters = parameters.Append( "&client_secret=" );
					parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( clientSecret, encoding ) );
					parameters = parameters.Append( "&grant_type=password" );
					parameters = parameters.Append( "&password=" );
					parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( password, encoding ) );
					parameters = parameters.Append( "&username=" );
					parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.Username, encoding ) );
					break;
				case LoginMode.RefreshToken:
					parameters = parameters.Append( "&grant_type=refresh_token" );
					parameters = parameters.Append( "&refresh_token=" );
					parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.RefreshToken, encoding ) );
					parameters = parameters.Append( "&redirect_uri=" );
					parameters = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.CallbackUrl, encoding ) );
					break;
				default:
					throw new System.InvalidOperationException( "Unknown LoginMode configured for the specified credential." );
			}
			return parameters.ToString();
		}
		private LoginResponse BuildLogin( System.Uri siteUrl, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.String, System.String>> headers, System.String body ) {
			var ssl = System.Net.SecurityProtocolType.Tls12;
#if DEBUG
			ssl = ssl | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
#endif
			System.Net.ServicePointManager.SecurityProtocol = ssl;

			using ( var client = new System.Net.WebClient {
				Encoding = System.Text.Encoding.UTF8
			} ) {
				foreach ( var header in headers ) {
					client.Headers[ header.Key ] = header.Value;
				}
				var rawResponse = client.UploadData( siteUrl, "POST", System.Text.Encoding.UTF8.GetBytes( body ) );
				var json = rawResponse.GetWebString(
					client.Encoding,
					client.ResponseHeaders.Keys.OfType<System.String>().Contains( "Content-Encoding" )
						? client.ResponseHeaders[ "Content-Encoding" ].TrimToNull() ?? "identity"
						: "identity"
				);
				dynamic respObj = Newtonsoft.Json.JsonConvert.DeserializeObject( json );
				return new LoginResponse( respObj );
			}
		}
		#endregion methods

	}

}
