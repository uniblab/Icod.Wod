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

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Pair<S, T> {

		#region fields
		private static readonly System.Int32 theHashCode;

		private readonly S myFirst;
		private readonly T mySecond;
		private readonly System.Int32 myHashCode;
		#endregion  fields


		#region .ctor
		static Pair() {
			theHashCode = 0;
			var aqn = System.Reflection.Assembly.GetExecutingAssembly().GetType().AssemblyQualifiedName;
			if ( !System.String.IsNullOrEmpty( aqn ) ) {
				theHashCode = aqn.GetHashCode();
			}
			aqn = typeof( S ).AssemblyQualifiedName;
			if ( !System.String.IsNullOrEmpty( aqn ) ) {
				unchecked {
					theHashCode += aqn.GetHashCode();
				}
			}
			aqn = typeof( T ).AssemblyQualifiedName;
			if ( !System.String.IsNullOrEmpty( aqn ) ) {
				unchecked {
					theHashCode += aqn.GetHashCode();
				}
			}
		}
		public Pair( S first, T second ) : base() {
			myFirst = first;
			mySecond = second;
			myHashCode = theHashCode;
			if ( first is not null ) {
				unchecked {
					myHashCode += first.GetHashCode();
				}
			}
			if ( second is not null ) {
				unchecked {
					myHashCode += second.GetHashCode();
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
		public sealed override System.Boolean Equals( System.Object? obj ) {
			return this.Equals( obj as Pair<S, T> );
		}
		public System.Boolean Equals( Pair<S, T>? other ) {
			if ( other is null ) {
				return false;
			} else if ( System.Object.ReferenceEquals( this, other ) ) {
				return true;
			} else {
				return System.Object.Equals( this.First, other.First )
					&& System.Object.Equals( this.Second, other.Second )
				;
			}
		}
		#endregion methods


		#region operators
		public static System.Boolean operator ==( Pair<S, T> a, Pair<S, T> b ) {
			if ( ( a is null ) && ( b is null ) ) {
				return true;
			} else if ( ( a is null ) || ( b is null ) ) {
				return false;
			} else {
				return a.Equals( b );
			}
		}
		public static System.Boolean operator !=( Pair<S, T> a, Pair<S, T> b ) {
			return !( a == b );
		}
		#endregion operators

	}

}
