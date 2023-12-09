// Copyright (C) 2023  Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public class JobInfoBase {

		#region .ctor
		protected JobInfoBase() : base() {
		}
		#endregion .ctor


		#region properties
		[Newtonsoft.Json.JsonProperty( propertyName: "columnDelimiter" )]
		public System.String ColumnDelimiter {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "contentType" )]
		public System.String ContentType {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "externalIdFieldName" )]
		public System.String ExternalIdFieldName {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "lineEnding" )]
		public System.String LineEnding {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "object" )]
		public System.String Object {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "operation" )]
		public System.String Operation {
			get;
			set;
		}
		[Newtonsoft.Json.JsonProperty( propertyName: "state" )]
		public System.String State {
			get;
			set;
		}
		#endregion properties

	}

}
