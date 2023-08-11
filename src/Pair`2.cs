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
