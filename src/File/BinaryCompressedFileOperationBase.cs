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

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( GZipFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DeflateFile ) )]
	[System.Xml.Serialization.XmlType(
		"binaryCompressedFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryCompressedFileOperationBase : BinaryFileOperationBase {

		#region fields
		private System.Boolean myDelete;
		private System.IO.Compression.CompressionMode myCompressionMode;
		#endregion fields


		#region .ctor
		protected BinaryCompressedFileOperationBase() : base() {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		protected BinaryCompressedFileOperationBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myCompressionMode = System.IO.Compression.CompressionMode.Decompress;
			myDelete = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"delete",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Delete {
			get {
				return myDelete;
			}
			set {
				myDelete = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"compressionMode",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( System.IO.Compression.CompressionMode.Decompress )]
		public System.IO.Compression.CompressionMode CompressionMode {
			get {
				return myCompressionMode;
			}
			set {
				myCompressionMode = value;
			}
		}
		#endregion properties

	}

}
