// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	public sealed class ColumnMap : System.IEquatable<ColumnMap> {

		#region fields
		private static readonly System.Collections.Generic.IEqualityComparer<ColumnMap> theValueComparer;
		private System.Boolean mySkip;
		#endregion fields


		#region .ctor
		static ColumnMap() {
			theValueComparer = ColumnMapValueComparer.Comparer;
		}

		public ColumnMap() : base() {
			mySkip = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public static System.Collections.Generic.IEqualityComparer<ColumnMap> ValueComparer {
			get {
				return theValueComparer;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"skip",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Skip {
			get {
				return mySkip;
			}
			set {
				mySkip = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"from",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String FromName {
			get;
			set;
		}
		[System.Xml.Serialization.XmlAttribute(
			"to",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String ToName {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override System.Int32 GetHashCode() {
			return base.GetHashCode();
		}

		public sealed override System.Boolean Equals( System.Object obj ) {
			if ( System.Object.ReferenceEquals( this, obj ) ) {
				return true;
			} else if ( null == (System.Object)obj ) {
				return false;
			}
			return this.Equals( obj as ColumnMap );
		}
		public System.Boolean Equals( ColumnMap other ) {
			if ( System.Object.ReferenceEquals( this, other ) ) {
				return true;
			} else if ( null == other ) {
				return false;
			} else {
				return System.String.Equals( this.FromName, other.FromName )
					&& System.String.Equals( this.ToName, other.ToName )
					&& this.Skip.Equals( other.Skip )
				;
			}
		}
		public static System.Boolean operator ==( ColumnMap x, ColumnMap y ) {
			if ( ( null == (System.Object)x ) && ( null == (System.Object)y ) ) {
				return true;
			} else if ( null != (System.Object)x ) {
				return x.Equals( y );
			} else {
				return y.Equals( x );
			}
		}
		public static System.Boolean operator !=( ColumnMap x, ColumnMap y ) {
			return !( x == y );
		}
		#endregion methods

	}

}
