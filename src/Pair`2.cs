// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Pair<S, T> {

		#region fields
		private readonly S myFirst;
		private readonly T mySecond;
		#endregion  fields


		#region .ctor
		public Pair( S first, T second ) : base() {
			myFirst = first;
			mySecond = second;
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

	}

}
