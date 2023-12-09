// Copyright (C) 2023  Timothy J. Bruce

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
		#endregion .ctor


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			return base.CreateJob( loginResponse, "insert" );
		}
		#endregion methods

	}

}
