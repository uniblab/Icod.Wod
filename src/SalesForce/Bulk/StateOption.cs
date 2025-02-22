// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class StateOption : System.IEquatable<StateOption>, System.IEquatable<System.String> {

		#region fields
		private static readonly System.Int32 theHashCode;

		private static readonly StateOption theAborted;
		private static readonly StateOption theFailed;
		private static readonly StateOption theInProgress;
		private static readonly StateOption theJobComplete;
		private static readonly StateOption theOpen;
		private static readonly StateOption theUploadComplete;

		private readonly System.String myValue;
		private readonly System.String myName;
		private readonly System.Int32 myHashcode;
		#endregion fields


		#region .ctor
		static StateOption() {
			theHashCode = typeof( StateOption ).GetHashCode();

			theAborted = new StateOption( JobState.Aborted );
			theFailed = new StateOption( JobState.Failed );
			theInProgress = new StateOption( JobState.InProgress );
			theJobComplete = new StateOption( JobState.JobComplete );
			theOpen = new StateOption( JobState.Open );
			theUploadComplete = new StateOption( JobState.UploadComplete );
		}

		private StateOption( System.String value ) : this( value, value ) {
		}
		private StateOption( System.String value, System.String name ) : base() {
			if ( System.String.IsNullOrEmpty( value ) ) {
				throw new System.ArgumentNullException( "value" );
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			myHashcode = theHashCode;
			myValue = value;
			if ( !System.String.IsNullOrEmpty( value ) ) {
				unchecked {
					myHashcode += value.GetHashCode();
				}
			}
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

		public static StateOption Aborted {
			get {
				return theAborted;
			}
		}
		public static StateOption Failed {
			get {
				return theFailed;
			}
		}
		public static StateOption InProgress {
			get {
				return theInProgress;
			}
		}
		public static StateOption JobComplete {
			get {
				return theJobComplete;
			}
		}
		public static StateOption Open {
			get {
				return theOpen;
			}
		}
		public static StateOption UploadComplete {
			get {
				return theUploadComplete;
			}
		}
		#endregion properties


		#region methods
		public sealed override System.String ToString() {
			return myValue;
		}
		public System.Boolean Equals( StateOption other ) {
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
				return this.Equals( ( obj as StateOption ) );
			}
		}
		public System.Boolean Equals( System.String other ) {
			return this.Value.Equals( other, System.StringComparison.OrdinalIgnoreCase );
		}
		public sealed override System.Int32 GetHashCode() {
			return myHashcode;
		}
		#endregion methods


		#region static methods
		public static StateOption FromName( System.String name ) {
			name = name.TrimToNull();
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			}
			if ( name.Equals( Aborted.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Aborted;
			} else if ( name.Equals( Failed.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Failed;
			} else if ( name.Equals( InProgress.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return InProgress;
			} else if ( name.Equals( JobComplete.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return JobComplete;
			} else if ( name.Equals( Open.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return Open;
			} else if ( name.Equals( UploadComplete.Name, System.StringComparison.OrdinalIgnoreCase ) ) {
				return UploadComplete;
			} else {
				throw new System.InvalidOperationException();
			}
		}

		public static System.Boolean operator ==( StateOption left, StateOption right ) {
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) || ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( StateOption left, StateOption right ) {
			return !( left == right );
		}

		public static explicit operator System.String( StateOption value ) { 
			return value?.Value;
		}

		public static System.Boolean operator ==( StateOption left, System.String right ) {
			if ( ( left is null ) && ( right is null ) ) {
				return true;
			} else if ( ( left is null ) || ( right is null ) ) {
				return false;
			} else {
				return left.Equals( right );
			}
		}
		public static System.Boolean operator !=( StateOption left, System.String right ) {
			return !( left == right );
		}
		#endregion static methods

	}

}