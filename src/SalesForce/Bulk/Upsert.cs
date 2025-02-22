// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkUpsert",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Upsert : AggregateMutationOperationBase {

		#region .ctor
		public Upsert() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"externalIdFieldName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String ExternalIdFieldName {
			get;
			set;
		}
		#endregion properties


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			var request = this.BuildSalesForceRequest( loginResponse, "application/json; charset=utf-8", "POST" );
			using ( var w = request.GetRequestStream() ) {
				var jr = new {
					operation = "upsert",
					externalIdFieldName = this.ExternalIdFieldName,
					columnDelimiter = "COMMA",
					contentType = "CSV",
#if DEBUG
					lineEnding = "CRLF",
#else
					lineEnding = "LF",
#endif
					@object = this.Object,
				};
				var js = Newtonsoft.Json.JsonConvert.SerializeObject( jr );
				var buffer = System.Text.Encoding.UTF8.GetBytes( js );
				w.Write( buffer, 0, buffer.Length );
			}
			using ( var response = (System.Net.HttpWebResponse)request.GetResponse() ) {
				var sc = response.StatusCode;
				if ( System.Net.HttpStatusCode.OK != sc ) {
					throw new System.Net.WebException( System.String.Format( "An invalid response was recevied from the server: {1} ({0})", response.StatusCode, response.StatusDescription ?? System.String.Empty ) );
				}
				return this.GetJobResponse( response );
			}
		}
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse, System.String operation ) {
			return this.CreateJob( loginResponse );
		}
		#endregion methods

	}

}
