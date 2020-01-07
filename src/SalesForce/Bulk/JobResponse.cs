using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class JobResponse : JobInfoBase {

		#region .ctor
		public JobResponse() : base() {
		}
		#endregion .ctor


		#region properties
		public System.Decimal ApiVersion {
			get;
			set;
		}
		public System.String ConcurrencyMode {
			get;
			set;
		}
		public System.String ContentUrl {
			get;
			set;
		}
		public System.String CreatedById {
			get;
			set;
		}
		public System.DateTime CreatedDate {
			get;
			set;
		}
		public System.String Id {
			get;
			set;
		}
		public JobType JobType {
			get;
			set;
		}
		public System.String State {
			get;
			set;
		}
		public System.DateTime SystemModstamp {
			get;
			set;
		}
		#endregion properties

	}

}