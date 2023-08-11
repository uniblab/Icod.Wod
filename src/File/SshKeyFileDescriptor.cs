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

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"sshKeyFileDescriptor",
		Namespace = "http://Icod.Wod"
	)]
	public class SshKeyFileDescriptor : FileDescriptor {

		#region fields
		private System.String myKeyFilePassword;
		#endregion fields


		#region .ctor
		public SshKeyFileDescriptor() : base() {
		}
		public SshKeyFileDescriptor( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"keyFilePassword",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String KeyFilePassword {
			get {
				return myKeyFilePassword;
			}
			set {
				myKeyFilePassword = value;
			}
		}
		#endregion properties

	}

}
