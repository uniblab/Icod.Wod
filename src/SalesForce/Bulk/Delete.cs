// Copyright 2023, Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sfBulkDelete",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Delete : AggregateMutationOperationBase {

		#region fields
		private const System.String theHardDelete = "hardDelete";
		private const System.String theDelete = "delete";

		private System.Boolean myIsHard;
		#endregion fields


		#region .ctor
		public Delete() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"hard",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean IsHard {
			get {
				return myIsHard;
			}
			set {
				myIsHard = value;
			}
		}
		#endregion properties


		#region methods
		protected sealed override JobResponse CreateJob( LoginResponse loginResponse ) {
			return this.IsHard
				? base.CreateJob( loginResponse, theHardDelete )
				: base.CreateJob( loginResponse, theDelete )
			;
		}
		#endregion methods

	}

}
