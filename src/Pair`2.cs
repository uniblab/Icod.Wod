// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Pair<S, T> : System.IEquatable<Pair<S, T>> {

		#region fields
		private static readonly System.Int32 theHashCode;

		private readonly S myFirst;
		private readonly T mySecond;
		private readonly System.Int32 myHashCode;
		#endregion  fields


		#region .ctor
		static Pair() {
			theHashCode = typeof( Pair<S, T> ).GetHashCode();
		}
		public Pair( S first, T second ) : base() {
			myFirst = first;
			mySecond = second;
			myHashCode = theHashCode;
			if ( myFirst is object ) {
				unchecked {
					myHashCode += myFirst.GetHashCode();
				}
			}
			if ( mySecond is object ) {
				unchecked {
					myHashCode += mySecond.GetHashCode();
				}
			}
		}
		#endregion .ctor


		#region properties
		public S First {
			get {
				return myFirst;
			}
		}
		public T Second {
			get {
				return mySecond;
			}
		}
		#endregion properties


		#region methods
		public sealed override System.Int32 GetHashCode() {
			return myHashCode;
		}
		public sealed override System.Boolean Equals( System.Object obj ) {
			if ( obj is null ) {
				return false;
			} else if ( ReferenceEquals( this, obj ) ) {
				return true;
			} else {
				return this.Equals( obj as Pair<S, T> );
			}
		}
		public System.Boolean Equals( Pair<S, T> other ) {
			if ( other is null ) {
				return false;
			} else if ( ReferenceEquals( this, other ) ) {
				return true;
			} else {
				return Equals( myFirst, other.First ) && Equals( mySecond, other.Second );
			}
		}
		#endregion methods


		#region static methods
		public static System.Boolean operator ==( Pair<S, T> left, Pair<S, T> right ) {
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) && ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( Pair<S, T> left, Pair<S, T> right ) {
			return !( left == right );
		}
		#endregion static methods

	}

}
