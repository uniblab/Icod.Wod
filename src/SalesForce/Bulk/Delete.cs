// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkDelete",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Delete : AggregateMutationOperationBase {

		#region .ctor
		public Delete() : base() {
		}
		public Delete( WorkOrder workOrder ) : base( workOrder ) {
		}
		public Delete( System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( missingSchemaAction, missingMappingAction ) {
		}
		public Delete( WorkOrder workOrder, System.Data.MissingSchemaAction missingSchemaAction, System.Data.MissingMappingAction missingMappingAction ) : base( workOrder, missingSchemaAction, missingMappingAction ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			return this.CreateJob( loginResponse, "delete" );
		}
		#endregion methods

	}

}
