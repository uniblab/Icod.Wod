using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public class JobInfoBase {

		#region .ctor
		protected JobInfoBase() : base() {
		}
		#endregion .ctor


		#region properties
		public System.String columnDelimiter {
			get;
			set;
		}
		public System.String contentType {
			get;
			set;
		}
		public System.String externalIdFieldName {
			get;
			set;
		}
		public System.String lineEnding {
			get;
			set;
		}
		public System.String @object {
			get;
			set;
		}
		public System.String operation {
			get;
			set;
		}
		public System.String state {
			get;
			set;
		}
		#endregion properties

	}

}