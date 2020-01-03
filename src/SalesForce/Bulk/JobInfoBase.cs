using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

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
		#endregion properties

	}

}