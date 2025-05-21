// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class LineEndingOption : System.IEquatable<LineEndingOption>, System.IEquatable<System.String> {

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
				throw new System.ArgumentNullException( nameof( value ) );
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( nameof( name ) );
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
			if ( other is null ) {
				return false;
			} else if ( ReferenceEquals( this, other ) ) {
				return true;
			} else {
				return this.Value.Equals( other.Value, System.StringComparison.OrdinalIgnoreCase );
			}
		}
		public sealed override System.Boolean Equals( System.Object obj ) {
			if ( obj is null ) {
				return false;
			} else if ( ReferenceEquals( this, obj ) ) {
				return true;
			} else {
				return this.Equals( ( obj as LineEndingOption ) );
			}
		}
		public System.Boolean Equals( System.String other ) {
			if ( System.String.IsNullOrEmpty( other ) ) {
				return false;
			} else if ( ReferenceEquals( this, other ) ) {
				return true;
			} else {
				return this.Value.Equals( other, System.StringComparison.OrdinalIgnoreCase );
			}
		}
		public sealed override System.Int32 GetHashCode() {
			return myHashcode;
		}
		#endregion methods


		#region static methods
		public static LineEndingOption FromName( System.String name ) {
			name = name.TrimToNull();
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( nameof( name ) );
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
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) || ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( LineEndingOption left, LineEndingOption right ) {
			return !( left == right );
		}

		public static System.Boolean operator ==( LineEndingOption left, System.String right ) {
			if ( ( left is null ) && ( System.String.IsNullOrEmpty( right ) ) ) {
				return true;
			} else if ( ( left is null ) || ( System.String.IsNullOrEmpty( right ) ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( LineEndingOption left, System.String right ) {
			return !( left == right );
		}
		#endregion static methods

	}

}
