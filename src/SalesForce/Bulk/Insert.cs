using System.Data.Common;
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkInsert",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Insert : AggregateMutationOperationBase {

		#region .ctor
		public Insert() : base() {
		}
		public Insert( WorkOrder workOrder ) : base( workOrder ) {
		}
		public Insert( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
		}
		public Insert( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			throw new System.NotImplementedException();
		}
		public sealed override void PerformWork( JobProcess jobProcess ) {
			var loginResponse = jobProcess.LoginResponse;
			var semaphore = jobProcess.Semaphore;

			throw new System.NotImplementedException();
		}
		#endregion methods

	}

}