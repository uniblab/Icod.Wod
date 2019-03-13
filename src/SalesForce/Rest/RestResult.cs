namespace Icod.Wod.SalesForce.Rest {

	[System.Serializable]
	public sealed class Result {

		#region .ctor
		public Result() : base() {
		}
		#endregion .ctor


		#region properties
		public System.Nullable<System.Int32> TotalSize {
			get;
			set;
		}
		public System.Nullable<System.Boolean> Done {
			get;
			set;
		}
		public System.String NextRecordsUrl {
			get;
			set;
		}
		public System.Object[] Records {
			get;
			set;
		}
		#endregion properties

	}

}