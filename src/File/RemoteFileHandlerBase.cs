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

namespace Icod.Wod.File {

	[System.Serializable]
	public abstract class RemoteFileHandlerBase : FileHandlerBase {

		#region .ctor
		protected RemoteFileHandlerBase() : base() {
		}
		protected RemoteFileHandlerBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		protected RemoteFileHandlerBase( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) { 
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public sealed override System.Char PathSeparator {
			get {
				return '/';
			}
		}
		#endregion properties


		#region methods
		protected virtual System.Collections.Generic.IEnumerable<System.String> ReadLines( System.Net.WebRequest request ) {
			System.Collections.Generic.ICollection<System.String> output = new System.Collections.Generic.List<System.String>();
			using ( var response = request.GetResponse() ) {
				using ( var stream = response.GetResponseStream() ) {
					using ( var reader = new System.IO.StreamReader( stream ) ) {
						System.String line = reader.ReadLine();
						while ( null != line ) {
							output.Add( line );
							line = reader.ReadLine();
						}
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}
