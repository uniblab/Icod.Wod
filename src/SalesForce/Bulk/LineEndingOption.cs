// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class LineEndingOption : System.IEquatable<LineEndingOption> {

		#region fields
		private static readonly LineEndingOption theLF;
		private static readonly LineEndingOption theCRLF;

		private readonly System.String myName;
		private readonly System.String myValue;
		private readonly System.Int32 myHashcode;
		#endregion fields


		#region .ctor
		static LineEndingOption() {
			theLF = new LineEndingOption( "\n", "LF" );
			theCRLF = new LineEndingOption( "\r\n", "CRLF" );
		}

		private LineEndingOption( System.String value, System.String name ) {
			if ( System.String.IsNullOrEmpty( value ) ) {
				throw new System.ArgumentNullException( "value" );
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			myValue = value;
			myHashcode = value.GetHashCode();
			myName = name;
		}
		#endregion .ctor


		#region properties
		public System.String Value {
			get {
				return myValue;
			}
		}
		public System.String Name {
			get {
				return myName;
			}
		}

		public static LineEndingOption LF {
			get {
				return theLF;
			}
		}
		public static LineEndingOption CRLF {
			get {
				return theCRLF;
			}
		}
		#endregion properties


		#region methods
		public sealed override System.String ToString() {
			return myValue;
		}
		public System.Boolean Equals( LineEndingOption other ) {
			return ( null == other )
				? false
				: System.Object.ReferenceEquals( this, other )
					? true
					: this.Value.Equals( other.Value, System.StringComparison.OrdinalIgnoreCase )
			;
		}
		public sealed override System.Boolean Equals( System.Object obj ) {
			return ( null == obj )
				? false
				: System.Object.ReferenceEquals( this, obj )
					? true
					: this.Equals( ( obj as LineEndingOption ) );
			;
		}
		public sealed override System.Int32 GetHashCode() {
			return myHashcode;
		}
		#endregion methods


		#region static methods
		public static LineEndingOption FromName( System.String name ) {
			name = name.TrimToNull();
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			if ( name.Equals( LF.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return LF;
			} else if ( name.Equals( CRLF.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return CRLF;
			} else {
				throw new System.InvalidOperationException();
			}
		}

		public static System.Boolean operator ==( LineEndingOption left, LineEndingOption right ) {
			return ( ( null == (System.Object)left ) && ( null == (System.Object)right ) )
				? true
				: ( null != (System.Object)left )
					? left.Equals( right )
					: right.Equals( left )
			;
		}
		public static System.Boolean operator !=( LineEndingOption left, LineEndingOption right ) {
			return !( left == right );
		}
		#endregion static methods

	}

}
