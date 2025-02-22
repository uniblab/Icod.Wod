// Copyright (C) 2025  Timothy J. Bruce

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
		#endregion .ctor


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			return base.CreateJob( loginResponse, "update" );
		}
		#endregion methods

	}

}
