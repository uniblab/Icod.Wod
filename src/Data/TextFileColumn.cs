// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public sealed class TextFileColumn : ColumnBase {

		#region fields
		private System.String myFormatString;
		#endregion fields


		#region .ctor
		public TextFileColumn() : base() {
			myFormatString = "{0}";
		}
		public TextFileColumn( System.String name ) : this() {
			this.Name = name;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"formatString",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "{0}" )]
		public System.String FormatString {
			get {
				return myFormatString;
			}
			set {
				myFormatString = value;
			}
		}
		#endregion properties

	
		#region methods
		public sealed override System.String GetColumnText( WorkOrder workOrder, System.Object value ) {
			return ( ( value is null ) || System.DBNull.Value.Equals( value ) )
				? this.NullReplacementText
				: workOrder.ExpandPseudoVariables( System.String.Format( workOrder.ExpandPseudoVariables( this.FormatString ) ?? "{0}", value ) )
			;
		}
		#endregion methods

	}

}
