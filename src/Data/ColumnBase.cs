// Icod.Wod.dll is the Work on Demand framework.
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
	[System.Xml.Serialization.XmlInclude( typeof( LiteralColumn ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TextFileColumn ) )]
	public abstract class ColumnBase {

		#region fields
		private System.Int32 myLength;
		#endregion fields


		#region .ctor
		protected ColumnBase() : base() {
			myLength = -1;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"name",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String Name {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"length",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( -1 )]
		public System.Int32 Length {
			get {
				return myLength;
			}
			set {
				myLength = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"nullReplacementText",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "" )]
		public virtual System.String NullReplacementText {
			get;
			set;
		}
		#endregion properties


		#region methods
		public abstract System.String? GetColumnText( WorkOrder workOrder, System.Object value );
		#endregion methods

	}

}
