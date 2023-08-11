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
	[System.Xml.Serialization.XmlType(
		"rmDir",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class RmDir : FileOperationBase {

		#region fields
		private System.Boolean myRecurse;
		#endregion fields


		#region .ctor
		public RmDir() : base() {
			myRecurse = false;
		}
		public RmDir( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myRecurse = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"recurse",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Recurse {
			get {
				return myRecurse;
			}
			set {
				myRecurse = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var fh = this.GetFileHandler( workOrder );
			foreach ( var fe in fh.ListDirectories() ) {
				fh.RmDir( fe.File, this.Recurse );
			}
		}
		#endregion methods

	}

}
