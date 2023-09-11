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
		"pruneFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class PruneFile : BinaryFileOperationBase {

		#region .ctor
		public PruneFile() : base() {
			this.TrimLines = true;
		}
		public PruneFile( WorkOrder workOrder ) : base( workOrder ) {
			this.TrimLines = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"trimLines",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean TrimLines {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			var sourceHandler = this.GetFileHandler( workOrder );
			var dest = this.Destination;
			if ( null == dest ) {
				dest = this;
			}
			var destHandler = dest.GetFileHandler( workOrder );

			var sourceEncoding = CodePageHelper.GetCodePage( this.CodePage );
			var destEncoding = CodePageHelper.GetCodePage( this.CodePage );

			System.Func<System.String, System.String?> trim;
			if ( this.TrimLines ) {
				trim = x => x.TrimToNull();
			} else {
				trim = x => x;
			}
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var source = sourceHandler.OpenReader( file ) ) {
						using ( var reader = new System.IO.StreamReader( source, sourceEncoding, true, this.BufferLength, true ) ) {
							using ( var writer = new System.IO.StreamWriter( buffer, destEncoding, this.BufferLength, true ) ) {
								var rs = this.RecordSeparator;
								System.String? line = reader.ReadLine( rs );
								while ( line is not null ) {
									line = trim( line );
									if ( !System.String.IsNullOrEmpty( line ) ) {
										writer.Write( line + rs );
									}
									line = reader.ReadLine( rs );
								}
							}
						}
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, file );
				}
			}
		}
		#endregion methods

	}

}
