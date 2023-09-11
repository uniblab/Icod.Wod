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

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( HeadFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TailFile ) )]
	[System.Xml.Serialization.XmlType(
		"countBinaryFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class CountBinaryFileOperationBase : BinaryFileOperationBase {

		#region fields
		private System.Int32 myCount;
		#endregion fields


		#region .ctor
		protected CountBinaryFileOperationBase() : base() {
			myCount = 0;
		}
		protected CountBinaryFileOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			myCount = 0;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"count",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Count {
			get {
				return myCount;
			}
			set {
				myCount = value;
			}
		}
		#endregion properties


		#region methods
		protected abstract IQueue<System.String> ReadPositiveCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding );
		protected abstract IQueue<System.String> ReadNegativeCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding );
		#endregion methods

	}

}
