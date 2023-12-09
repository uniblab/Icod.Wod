// Copyright 2023, Timothy J. Bruce

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
			} else if ( ( x is null ) || ( y is null ) ) {
				return false;
			} else {
				return x.Equals( y );
			}
		}

		public System.Int32 GetHashCode( ColumnMap obj ) {
			return ( obj is null ) 
				? 0 
				: obj.GetHashCode()
			;
		}
		#endregion methods

	}

}
