// Copyright 2023, Timothy J. Bruce
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
			if ( ( null == (System.Object)x ) && ( null == (System.Object)y ) ) {
				return true;
			} else if ( null != (System.Object)x ) {
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
