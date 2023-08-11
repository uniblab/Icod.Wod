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

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class ColumnDelimiterOption : System.IEquatable<ColumnDelimiterOption> {

		#region fields
		private static readonly ColumnDelimiterOption theBackquote;
		private static readonly ColumnDelimiterOption theCaret;
		private static readonly ColumnDelimiterOption theComma;
		private static readonly ColumnDelimiterOption thePipe;
		private static readonly ColumnDelimiterOption theSemicolon;
		private static readonly ColumnDelimiterOption theTab;

		private readonly System.Char myValue;
		private readonly System.String myName;
		private System.String? myString;
		private readonly System.Int32 myHashcode;
		#endregion fields


		#region .ctor
		static ColumnDelimiterOption() {
			theBackquote = new ColumnDelimiterOption( '\\', "BACKQUOTE" );
			theCaret = new ColumnDelimiterOption( '^', "CARET" );
			theComma = new ColumnDelimiterOption( ',', "COMMA" );
			thePipe = new ColumnDelimiterOption( '|', "PIPE" );
			theSemicolon = new ColumnDelimiterOption( ';', "SEMICOLON" );
			theTab = new ColumnDelimiterOption( '\t', "TAB" );
		}

		private ColumnDelimiterOption( System.Char value, System.String name ) {
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			myValue = value;
			myHashcode = System.Convert.ToInt32( value );
			myName = name;
		}
		#endregion .ctor


		#region properties
		public System.Char Value {
			get {
				return myValue;
			}
		}
		public System.String Name {
			get {
				return myName;
			}
		}

		public static ColumnDelimiterOption Backquote {
			get {
				return theBackquote;
			}
		}
		public static ColumnDelimiterOption Caret {
			get {
				return theCaret;
			}
		}
		public static ColumnDelimiterOption Comma {
			get {
				return theComma;
			}
		}
		public static ColumnDelimiterOption Pipe {
			get {
				return thePipe;
			}
		}
		public static ColumnDelimiterOption Semicolon {
			get {
				return theSemicolon;
			}
		}
		public static ColumnDelimiterOption Tab {
			get {
				return theTab;
			}
		}
		#endregion properties


		#region methods
		public sealed override System.String ToString() {
			if ( myString is null ) {
				_ = System.Threading.Interlocked.CompareExchange<System.String?>( ref myString, new System.String( myValue, 1 ), null );
			}
			return myString;
		}
		public System.Boolean Equals( ColumnDelimiterOption? other ) {
			return  ( other is not null )
				&& ( 
					ReferenceEquals( this, other ) || this.Value.Equals( other.Value )
				)
			;
		}
		public sealed override System.Boolean Equals( System.Object? obj ) {
			return ( obj is not null ) 
				&& ( 
					ReferenceEquals( this, obj ) || this.Equals( obj as ColumnDelimiterOption )
				)
			;
		}
		public sealed override System.Int32 GetHashCode() {
			return myHashcode;
		}
		#endregion methods


		#region static methods
		public static ColumnDelimiterOption FromName( System.String? name ) {
			name = name.TrimToNull();
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			if ( name.Equals( Backquote.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Backquote;
			} else if ( name.Equals( Caret.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Caret;
			} else if ( name.Equals( Comma.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Comma;
			} else if ( name.Equals( Pipe.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Pipe;
			} else if ( name.Equals( Semicolon.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Semicolon;
			} else if ( name.Equals( Tab.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Tab;
			} else {
				throw new System.InvalidOperationException();
			}
		}

		public static System.Boolean operator ==( ColumnDelimiterOption left, ColumnDelimiterOption right ) {
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) || ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( ColumnDelimiterOption left, ColumnDelimiterOption right ) {
			return !( left == right );
		}
		#endregion static methods

	}

}
