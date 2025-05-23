// Copyright (C) 2025  Timothy J. Bruce

using System.Collections.Generic;

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
		System.Boolean System.Collections.Generic.IEqualityComparer<ColumnMap>.Equals( ColumnMap x, ColumnMap y ) {
			return Equals( x, y );
		}
		System.Int32 IEqualityComparer<ColumnMap>.GetHashCode( ColumnMap obj ) {
			return GetHashCode( obj );
		}
		#endregion methods


		#region static methods
		public static System.Boolean Equals( ColumnMap x, ColumnMap y ) {
			if ( ( x is null ) && ( y is null ) ) {
				return true;
			} else if ( ( x is null ) || ( y is null ) ) {
				return false;
			} else {
				return x.Equals( y );
			}
		}
		public static System.Int32 GetHashCode( ColumnMap obj ) {
			return ( obj is null )
				? 0
				: obj.GetHashCode()
			;
		}
		#endregion static methods
	}

}
