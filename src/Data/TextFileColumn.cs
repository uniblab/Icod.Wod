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
		public sealed override System.String GetColumnText( System.Object value ) {
			return ( ( null == value ) || System.DBNull.Value.Equals( value ) )
				? this.NullReplacementText
				: System.String.Format( this.FormatString ?? "{0}", value )
			;
		}
		#endregion methods

	}

}