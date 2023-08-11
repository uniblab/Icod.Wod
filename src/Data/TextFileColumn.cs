// Icod.Wod is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

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
			return ( ( null == value ) || System.DBNull.Value.Equals( value ) )
				? this.NullReplacementText
				: workOrder.ExpandPseudoVariables( System.String.Format( workOrder.ExpandPseudoVariables( this.FormatString ) ?? "{0}", value ) )
			;
		}
		#endregion methods

	}

}
