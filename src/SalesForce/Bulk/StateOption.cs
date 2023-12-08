// Copyright 2022, Timothy J. Bruce
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Policy;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	public sealed class StateOption : System.IEquatable<StateOption>, System.IEquatable<System.String> {

		#region fields
		private const System.String theAbortedString = "Aborted";
		private static readonly StateOption theAborted;
		private const System.String theFailedString = "Failed";
		private static readonly StateOption theFailed;
		private const System.String theInProgressString = "InProgress";
		private static readonly StateOption theInProgress;
		private const System.String theJobCompleteString = "JobComplete";
		private static readonly StateOption theJobComplete;
		private const System.String theOpenString = "Open";
		private static readonly StateOption theOpen;
		private const System.String theUploadCompleteString = "UploadComplete";
		private static readonly StateOption theUploadComplete;

		private readonly System.String myValue;
		private readonly System.String myName;
		private readonly System.Int32 myHashcode;
		#endregion fields


		#region .ctor
		static StateOption() {
			theAborted = new StateOption( theAbortedString );
			theFailed = new StateOption( theFailedString );
			theInProgress = new StateOption( theInProgressString );
			theJobComplete = new StateOption( theJobCompleteString );
			theOpen = new StateOption( theOpenString );
			theUploadComplete = new StateOption( theUploadCompleteString );
		}

		private StateOption( System.String value ) : this( value, value ) {
		}
		private StateOption( System.String value, System.String name ) : base() {
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