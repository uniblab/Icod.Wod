// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Rest {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfRestSelect",
		Namespace = "http://Icod.Wod"
	)]
	public class RestSelect : SFOperationBase, Icod.Wod.Data.ITableSource, Icod.Wod.IStep {

		#region fields
		public static readonly System.Decimal DefaultApiVersion;

		private System.Decimal myApiVersion;
		private System.String myInstanceName;
		#endregion fields


		#region .ctor
		static RestSelect() {
			DefaultApiVersion = new System.Decimal( 47.0 );
		}

		public RestSelect() : base() {
			myApiVersion = DefaultApiVersion;
			myInstanceName = null;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"apiVersion",
			Namespace = "http://Icod.Wod"
		)]
		public System.Decimal ApiVersion {
			get {
				return myApiVersion;
			}
			set {
				myApiVersion = value;
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

		[System.Xml.Serialization.XmlAttribute(
			"soql",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String Soql {
			get;
			set;
		}

		[System.Xml.Serialization.XmlElement(
			"destination",
			Type = typeof( DbDestination ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public DbDestination Destination {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var dest = this.Destination ?? throw new System.InvalidOperationException();
			dest.WriteRecords( workOrder, this );
		}

		public System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var credential = Credential.GetCredential( this.InstanceName, workOrder );
			var loginToken = new Login() { WorkOrder = workOrder }.GetLoginResponse( credential, System.Text.Encoding.UTF8 );
			return this.ReadTables( loginToken );
		}
		public System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( LoginResponse loginToken ) {
			using ( var client = this.BuildClient( loginToken, this.WorkOrder.JobName ) ) {
				client.Headers[ "Content-type" ] = "application/x-www-form-urlencoded; charset=utf-8";
				var instanceUrl = new System.Uri( loginToken.InstanceUrl );
				var nextRecordsUrl = this.GetServicePath();
				var url = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, nextRecordsUrl ) {
					Query = this.GetQuery()
				}.Uri;
				System.Boolean done = true;
				System.String json = null;
				System.Data.DataRow row = null;
				System.Byte[] rawResponse = null;
				do {
					rawResponse = client.DownloadData( url );
					json = ( client.ResponseHeaders.Keys.OfType<System.String>().Contains( "Content-Encoding" ) && client.ResponseHeaders[ "Content-Encoding" ].Equals( "gzip", System.StringComparison.OrdinalIgnoreCase ) )
						? StringHelper.Gunzip( rawResponse, client.Encoding )
						: client.Encoding.GetString( rawResponse )
					;
					dynamic response = Newtonsoft.Json.Linq.JObject.Parse( json );
					dynamic records = response.records;
					using ( var table = new System.Data.DataTable() ) {
						dynamic probe = records.First;
						foreach ( var colName in ( probe as System.Collections.Generic.IDictionary<System.String, Newtonsoft.Json.Linq.JToken> ).Where(
							x => !x.Value.HasValues
						).Select(
							x => x.Key
						) ) {
							table.Columns.Add( new System.Data.DataColumn( colName ) );
						}
						foreach ( var record in records ) {
							row = table.NewRow();
							foreach ( var col in table.Columns.OfType<System.Data.DataColumn>().Select(
								x => x.ColumnName
							) ) {
								row[ col ] = record[ col ].Value;
							}
							table.Rows.Add( row );
						}
						yield return table;
					}
					done = (System.Boolean)response.done;
					if ( !done ) {
						nextRecordsUrl = (System.String)response.nextRecordsUrl;
						url = new System.UriBuilder( instanceUrl.Scheme, instanceUrl.Host, instanceUrl.Port, nextRecordsUrl ).Uri;
					}
				} while ( !done );
			}
		}

		private System.String GetQuery() {
			var soql = this.Soql;
			if ( System.String.IsNullOrEmpty( soql ) ) {
				throw new System.InvalidOperationException();
			}
			return "q=" + System.Web.HttpUtility.UrlEncode( soql, System.Text.Encoding.UTF8 );
		}
		private System.String GetServicePath() {
			return System.String.Format( "/services/data/v{0:F1}/query/", this.ApiVersion );
		}

		private System.Net.WebClient BuildClient( LoginResponse token, System.String userAgent ) {
			if ( token is null ) {
				throw new System.ArgumentNullException( nameof( token ) );
			}
			var client = new System.Net.WebClient {
				Encoding = System.Text.Encoding.UTF8
			};
			client.Headers[ "Authorization" ] = "Bearer " + token.AccessToken;
			client.Headers[ "User-Agent" ] = userAgent.TrimToNull() ?? System.Reflection.Assembly.GetExecutingAssembly().FullName;
			System.Net.ServicePointManager.SecurityProtocol = TlsHelper.GetSecurityProtocol();
			return client;
		}
		#endregion methods

	}

}
