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

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public sealed class ColumnMapValueComparer : System.Collections.Generic.IEqualityComparer<ColumnMap> {

		#region fields
		private static readonly System.Collections.Generic.IEqualityComparer<ColumnMap> theValueComparer;
		#endregion fields


		#region .ctor
		static ColumnMapValueComparer() {
			theValueComparer = new ColumnMapValueComparer();
		}

		private ColumnMapValueComparer() :  base() {
		}
		#endregion .ctor


		#region properties
		public static System.Collections.Generic.IEqualityComparer<ColumnMap> Comparer {
			get {
				return theValueComparer;
			}
		}
		#endregion properties


		#region methods
		public System.Boolean Equals( ColumnMap x, ColumnMap y ) {
			if ( ( x is null ) && ( y is null ) ) {
				return true;
			} else if ( x is object ) {
				return x.Equals( y );
			} else {
				return y.Equals( x );
			}
		}

		public System.Int32 GetHashCode( ColumnMap obj ) {
			return ( null == obj ) ? 0 : obj.GetHashCode();
		}
		#endregion methods

	}

}
