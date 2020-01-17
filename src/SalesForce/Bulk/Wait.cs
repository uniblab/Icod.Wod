using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	public sealed class Wait {

		#region fields
		private const System.Int32 InitialWaitMiliseconds = 5000;
		private const System.Int32 MinimumWaitMiliseconds = 5000;
		private const System.Int32 IncrementMiliseconds = 1000;
		private const System.Int32 MaxWaitMiliseconds = 45000;

		private System.Int32 myInitial;
		private System.Int32 myMinimum;
		private System.Int32 myIncrement;
		private System.Int32 myMaximum;
		#endregion fields


		#region .ctor
		public Wait() : base() {
			myInitial = InitialWaitMiliseconds;
			myMinimum = MinimumWaitMiliseconds;
			myIncrement = IncrementMiliseconds;
			myMaximum = MaxWaitMiliseconds;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"initial",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( InitialWaitMiliseconds )]
		public System.Int32 Initial {
			get {
				return myInitial;
			}
			set {
				if ( value <= 0 ) {
					throw new System.InvalidOperationException( "Wait times must be positive." );
				}
				myInitial = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"minimum",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( MinimumWaitMiliseconds )]
		public System.Int32 Minimum {
			get {
				return myMinimum;
			}
			set {
				if ( value <= 0 ) {
					throw new System.InvalidOperationException( "Wait times must be positive." );
				}
				myMinimum = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"increment",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( IncrementMiliseconds )]
		public System.Int32 Increment {
			get {
				return myIncrement;
			}
			set {
				if ( value <= 0 ) {
					throw new System.InvalidOperationException( "Wait times must be positive." );
				}
				myIncrement = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"maximum",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( MaxWaitMiliseconds )]
		public System.Int32 Maximum {
			get {
				return myMaximum;
			}
			set {
				if ( value <= 0 ) {
					throw new System.InvalidOperationException( "Wait times must be positive." );
				}
				myMaximum = value;
			}
		}
		#endregion properties

	}

}