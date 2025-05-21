// Copyright (C) 2025  Timothy J. Bruce
using System.Data.SqlClient;
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
			clientId = clientId ?? throw new System.ArgumentNullException( nameof( clientId ) );
			var credential = Credential.GetCredential( clientId, this.WorkOrder );
			return this.GetLoginResponse( credential, System.Text.Encoding.UTF8 );
		}
		public LoginResponse GetLoginResponse( SalesForce.ICredential credential, System.Text.Encoding encoding ) {
			credential = credential ?? throw new System.ArgumentNullException( nameof( credential ) );
			if (
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
			credential = credential ?? throw new System.ArgumentNullException( nameof( credential ) );

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
			credential = credential ?? throw new System.ArgumentNullException( nameof( credential ) );

			var clientId = credential.ClientId;
			var parameters = new System.Text.StringBuilder();
			var encoding = System.Text.Encoding.UTF8;
			_ = parameters.Append( "client_id=" );
			_ = parameters.Append( System.Web.HttpUtility.UrlEncode( clientId, encoding ) );
			switch ( credential.LoginMode ) {
				case LoginMode.Password:
					var clientSecret = credential.ClientSecret;
					var password = ( credential.Password ?? System.String.Empty ) + ( credential.SecurityToken ?? System.String.Empty );
					if ( System.String.IsNullOrEmpty( password ) ) {
						throw new System.InvalidOperationException( "The specified credential is attempting Password authentication but does not have a password configured." );
					}
					_ = parameters.Append( "&client_secret=" );
					_ = parameters.Append( System.Web.HttpUtility.UrlEncode( clientSecret, encoding ) );
					_ = parameters.Append( "&grant_type=password" );
					_ = parameters.Append( "&password=" );
					_ = parameters.Append( System.Web.HttpUtility.UrlEncode( password, encoding ) );
					_ = parameters.Append( "&username=" );
					_ = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.Username, encoding ) );
					break;
				case LoginMode.RefreshToken:
					_ = parameters.Append( "&grant_type=refresh_token" );
					_ = parameters.Append( "&refresh_token=" );
					_ = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.RefreshToken, encoding ) );
					_ = parameters.Append( "&redirect_uri=" );
					_ = parameters.Append( System.Web.HttpUtility.UrlEncode( credential.CallbackUrl, encoding ) );
					break;
				default:
					throw new System.InvalidOperationException( "Unknown LoginMode configured for the specified credential." );
			}
			return parameters.ToString();
		}
		private LoginResponse BuildLogin( System.Uri siteUrl, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.String, System.String>> headers, System.String body ) {
			System.Net.ServicePointManager.SecurityProtocol = TlsHelper.GetSecurityProtocol();

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
