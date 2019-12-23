namespace Icod.Wod.SalesForce {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( Rest.RestSelect ) )]
	[System.Xml.Serialization.XmlType(
		"sfOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class SFOperationBase {

		#region fields
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		private System.String myInstanceName;
		#endregion fields


		#region .ctor
		protected SFOperationBase() : base() {
			myWorkOrder = null;
			myInstanceName = null;
		}
		protected SFOperationBase( WorkOrder workOrder ) : this() {
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

		[System.Xml.Serialization.XmlAttribute(
			"instanceName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String InstanceName {
			get {
				return myInstanceName;
			}
			set {
				myInstanceName = value.TrimToNull();
			}
		}
		#endregion properties


		#region static methods
		protected static System.Net.WebClient BuildClient( LoginResponse token, System.String userAgent ) {
			if ( null == token ) {
				throw new System.ArgumentNullException( "token" );
			}
			var client = new System.Net.WebClient {
				Encoding = System.Text.Encoding.UTF8
			};
			client.Headers[ "Authorization" ] = "Bearer " + token.AccessToken;
			client.Headers[ "User-Agent" ] = userAgent.TrimToNull() ?? System.Reflection.Assembly.GetExecutingAssembly().FullName;
#if DEBUG
			client.Headers[ "Accept-Encoding" ] = "identity, gzip";
			System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Ssl3;
#else
			client.Headers[ "Accept-Encoding" ] = "gzip, identity";
			System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
#endif
			return client;
		}
		#endregion static methods

	}

}