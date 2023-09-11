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

	[System.Xml.Serialization.XmlInclude( typeof( AppendFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CopyFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( JsonToXml ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PreambleFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PrefixFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PruneFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RebaseFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RenameFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( SuffixFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( XmlToJson ) )]
	[System.Xml.Serialization.XmlInclude( typeof( XmlTransformFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryCompressedFileOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CountBinaryFileOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileOrDirectoryListerBase ) )]
	[System.Xml.Serialization.XmlType(
		"binaryFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryFileOperationBase : FileOperationBase, IDestination {

		#region .ctor
		protected BinaryFileOperationBase() : base() {
		}
		protected BinaryFileOperationBase( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		[System.ComponentModel.DefaultValue( null )]
		public FileDescriptor? Destination {
			get;
			set;
		}
		#endregion properties

	}

}
