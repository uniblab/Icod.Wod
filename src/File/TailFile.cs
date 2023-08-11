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
		"tailFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class TailFile : CountBinaryFileOperationBase {

		#region .ctor
		public TailFile() : base() {
		}
		public TailFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceHandler = this.GetFileHandler( workOrder );
			if ( null == sourceHandler ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination;
			if ( null == dest ) {
				dest = this;
			}
			var destHandler = dest.GetFileHandler( workOrder );

			System.Func<FileHandlerBase, System.String, System.Text.Encoding, IQueue<System.String>> reader = null;
			var count = this.Count;
			if ( 0 == count ) {
				throw new System.InvalidOperationException( "Count may not be 0." );
			} else if ( 0 < count ) {
				reader = this.ReadPositiveCount;
			} else {
				reader = this.ReadNegativeCount;
			}
			var sourceEncoding = this.GetEncoding();
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var writer = new System.IO.StreamWriter( buffer, sourceEncoding, this.BufferLength, true ) ) {
						var rs = this.RecordSeparator;
						foreach ( var line in reader( sourceHandler, file, sourceEncoding ) ) {
							writer.Write( line + rs );
						}
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, dest.GetFilePathName( destHandler, file ) );
				}
			}
		}
		protected sealed override IQueue<System.String> ReadPositiveCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Queue<System.String>.Empty;
			System.String line = null;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					line = reader.ReadLine( rs );
					while ( null != line ) {
						output = output.Enqueue( line );
						line = reader.ReadLine( rs );
					}
				}
			}
			var count = this.Count;
			for ( var i = 0; i < count; i++ ) {
				if ( output.IsEmpty ) {
					break;
				}
				output = output.Dequeue();
			}
			return output;
		}
		protected sealed override IQueue<System.String> ReadNegativeCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Queue<System.String>.Empty;
			System.String line = null;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					var count = -this.Count;
					for ( var i = 0; i < count; i++ ) {
						line = reader.ReadLine( rs );
						if ( null == line ) {
							break;
						}
					}
					line = reader.ReadLine();
					while ( null != line ) {
						output = output.Enqueue( line );
						line = reader.ReadLine( rs );
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}
