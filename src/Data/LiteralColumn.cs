using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public sealed class LiteralColumn : ColumnBase {

		#region .ctor
		public LiteralColumn() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"value",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String Value {
			get;
			set;
		}
		#endregion properties

		#region methods
		public sealed override System.String GetColumnText( WorkOrder workOrder, System.Object value ) {
			return workOrder.ExpandPseudoVariables( this.Value );
		}
		#endregion methods

	}

}