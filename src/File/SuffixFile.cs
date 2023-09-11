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
		"suffixFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class SuffixFile : BinaryFileOperationBase {

		#region .ctor
		public SuffixFile() : base() {
		}
		public SuffixFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion  .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"suffix",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Suffix {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			var sourceHandler = this.GetFileHandler( workOrder );
			if ( null == sourceHandler ) {
				throw new System.InvalidOperationException();
			}
			var suffix = this.Suffix;
			if ( System.String.IsNullOrEmpty( suffix ) ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination;
			if ( null == dest ) {
				dest = this;
			}
			var destHandler = dest.GetFileHandler( workOrder );

			var rs = this.RecordSeparator;
			System.Action<System.IO.StreamWriter, System.IO.StreamReader, System.String> worker;
			if ( System.String.IsNullOrEmpty( rs ) ) {
				worker = this.SuffixOnce;
			} else {
				worker = this.SuffixEach;
			}
			var sourceEncoding = this.GetEncoding();
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var writer = new System.IO.StreamWriter( buffer, sourceEncoding, this.BufferLength, true ) ) {
						using ( var original = sourceHandler.OpenReader( file ) ) {
							using ( var reader = new System.IO.StreamReader( original, this.GetEncoding(), true, this.BufferLength, true ) ) {
								worker( writer, reader, suffix );
							}
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, dest.GetFilePathName( destHandler, file ) );
				}
			}
		}
		private void SuffixOnce( System.IO.StreamWriter destination, System.IO.StreamReader source, System.String suffix ) {
			if ( System.String.IsNullOrEmpty( suffix ) ) {
				throw new System.ArgumentNullException( nameof( suffix ) );
			}
			source.BaseStream.CopyTo( destination.BaseStream );
			destination.Flush();
			destination.Write( suffix );
		}
		private void SuffixEach( System.IO.StreamWriter destination, System.IO.StreamReader source, System.String suffix ) {
			if ( System.String.IsNullOrEmpty( suffix ) ) {
				throw new System.ArgumentNullException( nameof( suffix ) );
			}
			System.String? line;
			while ( !source.EndOfStream ) {
				line = source.ReadLine( this.RecordSeparator );
				if ( !System.String.IsNullOrEmpty( line ) ) {
					destination.Write( line );
					destination.Write( suffix );
					destination.Write( this.RecordSeparator );
				}
			}
		}
		#endregion methods

	}

}
