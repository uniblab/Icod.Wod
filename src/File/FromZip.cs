using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fromZip",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FromZip : FileOperationBase {

		#region fields
		private System.Boolean myTruncateEntryName;
		#endregion fields


		#region .ctor
		public FromZip() : base() {
			myTruncateEntryName = true;
		}
		public FromZip( WorkOrder workOrder ) : base( workOrder ) {
			myTruncateEntryName = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"truncateEntryName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean TruncateEntryName {
			get {
				return myTruncateEntryName;
			}
			set {
				myTruncateEntryName = value;
			}
		}


		[System.Xml.Serialization.XmlElement(
			"source",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Source {
			get;
			set;
		}
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
		}
		#endregion methods

	}

}