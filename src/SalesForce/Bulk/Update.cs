using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkUpdate",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Update : AggregateMutationOperationBase {

		#region .ctor
		public Update() : base() {
		}
		public Update( WorkOrder workOrder ) : base( workOrder ) {
		}
		public Update( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
		}
		public Update( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			return this.CreateJob( loginResponse, "update" );
		}
		#endregion methods

	}

}
