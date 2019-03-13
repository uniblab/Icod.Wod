using System.Linq;

namespace Icod.Wod.SalesForce.Rest {

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
		#endregion properties


		#region methods
		private ICredential GetCredential( System.String clientId ) {
			ICredential here = null;
			if ( null != myWorkOrder ) {
				here = ( myWorkOrder.SFCredentials ?? new ICredential[ 0 ] ).FirstOrDefault(
					x => x.ClientId.Equals( clientId, System.StringComparison.OrdinalIgnoreCase )
				);
			}
			if ( null != here ) {
				return here;
			}
			var section = (Configuration.SalesForceCredentialSection)Configuration.SalesForceCredentialSection.GetSection();
			if ( null == section ) {
				throw new System.Configuration.ConfigurationErrorsException( "The icod.wod.sfCredentials is not defined." );
			}
			var collection = section.Credentials;
			if ( ( null == collection ) || ( collection.Count < 1 ) ) {
				throw new System.Configuration.ConfigurationErrorsException( "No such configuration element is defined." );
			}
			return collection[ clientId ];
		}
		public LoginResponse GetLoginResponse( System.String clientId ) {
			if ( System.String.IsNullOrEmpty( clientId ) ) {
				throw new System.ArgumentNullException( "clientId" );
			}
			var credential = this.GetCredential( clientId );
			return this.GetLoginResponse( credential );
		}
		public LoginResponse GetLoginResponse( SalesForce.ICredential credential ) {
			if ( null == credential ) {
				throw new System.ArgumentNullException( "element" );
			}
			var clientId = credential.ClientId;
			var clientSecret = credential.ClientSecret;
			var username = credential.Username;
			var password = credential.Password + credential.SecurityToken;
			var parameters = new System.Text.StringBuilder();
			var encoding = System.Text.Encoding.UTF8;
			parameters.Append( "grant_type=password" );
			parameters.Append( "&client_id=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( clientId, encoding ) );
			parameters.Append( "&client_secret=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( clientSecret, encoding ) );
			parameters.Append( "&username=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( username, encoding ) );
			parameters.Append( "&password=" );
			parameters.Append( System.Web.HttpUtility.UrlEncode( password, encoding ) );
			System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
			using ( var client = new System.Net.WebClient() ) {
				client.Encoding = encoding;
				client.Headers[ "Content-type" ] = "application/x-www-form-urlencoded";
				client.Headers[ "Accept-Encoding" ] = "identity";
				var rawResponse = client.UploadString( credential.SiteUrl, parameters.ToString() );
				dynamic respObj = Newtonsoft.Json.JsonConvert.DeserializeObject( rawResponse );
				return new LoginResponse( respObj );
			}
		}
		#endregion methods

	}

}