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
		public WorkOrder WorkOrder {
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
			return this.GetLoginResponse( credential );
		}
		public LoginResponse GetLoginResponse( SalesForce.ICredential credential ) {
			if ( null == credential ) {
				throw new System.ArgumentNullException( "element" );
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
			var clientId = credential.ClientId;
			var clientSecret = credential.ClientSecret;
			var parameters = new System.Text.StringBuilder();
			var encoding = System.Text.Encoding.UTF8;
			parameters.Append( "client_id=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( clientId, encoding ) );
			parameters.Append( "&client_secret=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( clientSecret, encoding ) );
			switch ( credential.LoginMode ) {
				case LoginMode.Password:
					var password = ( credential.Password ?? System.String.Empty ) + ( credential.SecurityToken ?? System.String.Empty );
					if ( System.String.IsNullOrEmpty( password ) ) {
						throw new System.InvalidOperationException( "The specified credential is attempting Password authentication but does not have a password configured." );
					}
					parameters.Append( "&password=" );
					parameters.Append( System.Web.HttpUtility.UrlEncode( password, encoding ) );
					parameters.Append( "&grant_type=password" );
					parameters.Append( "&username=" );
					parameters.Append( System.Web.HttpUtility.UrlEncode( credential.Username, encoding ) );
					break;
				case LoginMode.RefreshToken:
					parameters.Append( "&grant_type=refresh_token" );
					parameters.Append( "&refresh_token=" );
					parameters.Append( System.Web.HttpUtility.UrlEncode( credential.RefreshToken, encoding ) );
					parameters.Append( "&redirect_uri=" );
					parameters.Append( System.Web.HttpUtility.UrlEncode( credential.CallbackUrl, encoding ) );
					break;
				default:
					throw new System.InvalidOperationException( "Unknown LoginMode configured for the specified credential." );
			}
			var body = parameters.ToString();

			using ( var client = new System.Net.WebClient() ) {
				client.Headers[ "User-Agent" ] = "Icod Work on Demand Framework, using .Net System.Net.WebClient";
				client.Encoding = encoding;
				client.Headers[ "Content-type" ] = "application/x-www-form-urlencoded";
#if DEBUG
				client.Headers[ "Accept-Encoding" ] = "identity, gzip";
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
#else
				client.Headers[ "Accept-Encoding" ] = "gzip, identity";
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
#endif
				var rawResponse = client.UploadData( credential.SiteUrl, System.Text.Encoding.UTF8.GetBytes( body ) );
				var json = ( client.ResponseHeaders.Keys.OfType<System.String>().Contains( "Content-Encoding" ) && client.ResponseHeaders[ "Content-Encoding" ].Equals( "gzip", System.StringComparison.OrdinalIgnoreCase ) )
					? StringHelper.Gunzip( rawResponse, client.Encoding )
					: client.Encoding.GetString( rawResponse )
				;
				dynamic respObj = Newtonsoft.Json.JsonConvert.DeserializeObject( json );
				return new LoginResponse( respObj );
			}
		}
		#endregion methods

	}

}