// Icod.Wod is the Work on Demand framework.
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
			return !( other is null ) 
				&& ( 
					ReferenceEquals( this, other ) 
					|| this.Value.Equals( other.Value, System.StringComparison.OrdinalIgnoreCase )
				)
			;
		}
		public sealed override System.Boolean Equals( System.Object obj ) {
			return !( obj is null ) 
				&& ( 
					ReferenceEquals( this, obj ) || this.Equals( obj as LineEndingOption ) 
				)
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
		#endregion static methods

	}

}
