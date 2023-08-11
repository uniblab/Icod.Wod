
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
	public sealed class ColumnMap : System.IEquatable<ColumnMap> {

		#region fields
		private static readonly System.Collections.Generic.IEqualityComparer<ColumnMap> theValueComparer;
		private System.Boolean mySkip;
		#endregion fields


		#region .ctor
		static ColumnMap() {
			theValueComparer = ColumnMapValueComparer.Comparer;
		}

		public ColumnMap() : base() {
			mySkip = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public static System.Collections.Generic.IEqualityComparer<ColumnMap> ValueComparer {
			get {
				return theValueComparer;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"skip",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Skip {
			get {
				return mySkip;
			}
			set {
				mySkip = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"from",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String FromName {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"to",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String ToName {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override System.Int32 GetHashCode() {
			return base.GetHashCode();
		}

		public sealed override System.Boolean Equals( System.Object obj ) {
			if ( System.Object.ReferenceEquals( this, obj ) ) {
				return true;
			} else if ( obj is null ) {
				return false;
			}
			return this.Equals( obj as ColumnMap );
		}
		public System.Boolean Equals( ColumnMap other ) {
			if ( System.Object.ReferenceEquals( this, other ) ) {
				return true;
			} else if ( null == other ) {
				return false;
			} else {
				return System.String.Equals( this.FromName, other.FromName )
					&& System.String.Equals( this.ToName, other.ToName )
					&& this.Skip.Equals( other.Skip )
				;
			}
		}
		public static System.Boolean operator ==( ColumnMap left, ColumnMap right ) {
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) || ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( ColumnMap x, ColumnMap y ) {
			return !( x == y );
		}
		#endregion methods

	}

}
