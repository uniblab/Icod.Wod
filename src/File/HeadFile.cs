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
	[System.Xml.Serialization.XmlType(
		"headFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class HeadFile : CountBinaryFileOperationBase {

		#region .ctor
		public HeadFile() : base() {
		}
		public HeadFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override IQueue<System.String> ReadPositiveCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Queue<System.String>.Empty;
			var count = this.Count;
			System.String? line;
			System.Int32 lineCount = 0;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					line = reader.ReadLine( rs );
					lineCount++;
					while ( line is not null ) {
						output = output.Enqueue( line );
						line = reader.ReadLine( rs );
						lineCount++;
						if ( count < lineCount ) {
							break;
						}
					}
				}
			}
			return output;
		}
		protected sealed override IQueue<System.String> ReadNegativeCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Stack<System.String>.Empty;
			System.String? line;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					line = reader.ReadLine( rs );
					while ( line is not null ) {
						output = output.Push( line );
						line = reader.ReadLine( rs );
					}
				}
			}
			var count = -this.Count;
			for ( var i = 0; i < count; i++ ) {
				if ( output.IsEmpty ) {
					break;
				}
				output = output.Pop();
			}
			return output.ToQueue();
		}
		#endregion methods

	}

}
