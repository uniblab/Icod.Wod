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

namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlInclude( typeof( DeleteFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ExecuteFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ExistsFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( MkDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RmDir ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TouchFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryFileOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ZipOperationBase ) )]
	[System.Xml.Serialization.XmlType(
		"fileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class FileOperationBase : FileBase, IStep {

		#region .ctor
		protected FileOperationBase() : base() {
		}
		protected FileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public abstract void DoWork( WorkOrder workOrder );
		#endregion methods

	}

}
