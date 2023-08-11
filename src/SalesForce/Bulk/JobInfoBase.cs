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
	public class JobInfoBase {

		#region .ctor
		protected JobInfoBase() : base() {
		}
		#endregion .ctor


		#region properties
		public System.String columnDelimiter {
			get;
			set;
		}
		public System.String contentType {
			get;
			set;
		}
		public System.String externalIdFieldName {
			get;
			set;
		}
		public System.String lineEnding {
			get;
			set;
		}
		public System.String @object {
			get;
			set;
		}
		public System.String operation {
			get;
			set;
		}
		public System.String state {
			get;
			set;
		}
		#endregion properties

	}

}
