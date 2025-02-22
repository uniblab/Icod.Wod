// Copyright (C) 2025  Timothy J. Bruce
namespace Icod.Wod.SalesForce {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( Rest.RestSelect ) )]
	[System.Xml.Serialization.XmlInclude( typeof( Bulk.BulkAggregateOperation ) )]
	[System.Xml.Serialization.XmlType(
		"sfOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class SFOperationBase {

		#region fields
		[System.NonSerialized]
		private Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		protected SFOperationBase() : base() {
			myWorkOrder = null;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public Icod.Wod.WorkOrder WorkOrder {
			get {
				return myWorkOrder;
			}
			set {
				myWorkOrder = value;
			}
		}
		#endregion properties

	}

}
