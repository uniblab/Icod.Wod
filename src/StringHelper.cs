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

namespace Icod.Wod {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class StringHelper {

		public static System.String TrimToNull( this System.String @string ) {
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = @string.Trim();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return @string;
		}

		public static System.Byte[] Compress(
			this System.String @string,
			System.Text.Encoding encoding,
			System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> compressor
		) {
			using ( var input = new System.IO.MemoryStream( encoding.GetBytes( @string ), false ) ) {
				using ( var worker = compressor( input, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var output = new System.IO.MemoryStream() ) {
						worker.CopyTo( output );
						output.Flush();
						_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
						return output.ToArray();
					}
				}
			}
		}
		public static System.Byte[] Gzip( this System.String @string, System.Text.Encoding encoding ) {
			return @string.Compress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.GZipStream( stream, compressionMode, leaveOpen )
			);
		}
		public static System.Byte[] Deflate( this System.String @string, System.Text.Encoding encoding ) {
			return @string.Compress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.DeflateStream( stream, compressionMode, leaveOpen )
			);
		}

		public static System.String GetString( this System.Byte[] response, System.Text.Encoding encoding ) {
			return encoding.GetString( response );
		}
		public static System.String Decompress(
			this System.Byte[] response,
			System.Text.Encoding encoding,
			System.Func<System.IO.Stream, System.IO.Compression.CompressionMode, System.Boolean, System.IO.Stream> decompressor
		) {
			using ( var input = new System.IO.MemoryStream( response, false ) ) {
				using ( var worker = decompressor( input, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var output = new System.IO.MemoryStream() ) {
						worker.CopyTo( output );
						output.Flush();
						_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
						return output.ToArray().GetString( encoding );
					}
				}
			}
		}
		public static System.String Gunzip( this System.Byte[] response, System.Text.Encoding encoding ) {
			return response.Decompress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.GZipStream( stream, compressionMode, leaveOpen )
			);
		}
		public static System.String Inflate( this System.Byte[] response, System.Text.Encoding encoding ) {
			return response.Decompress(
				encoding,
				( stream, compressionMode, leaveOpen ) => new System.IO.Compression.DeflateStream( stream, compressionMode, leaveOpen )
			);
		}

		public static System.String GetWebString( this System.Byte[] response, System.Text.Encoding encoding, System.String contentEncoding ) {
			return ( contentEncoding.TrimToNull() ?? "identity" ).Equals( "identity", System.StringComparison.OrdinalIgnoreCase )
				? response.GetString( encoding )
				: contentEncoding.Equals( "gzip", System.StringComparison.OrdinalIgnoreCase )
					? response.Gunzip( encoding )
					: contentEncoding.Equals( "deflate", System.StringComparison.OrdinalIgnoreCase )
						? response.Inflate( encoding )
						: throw new System.InvalidOperationException( System.String.Format(
							"Unknown Content-Encoding value received from server: {0}",
							contentEncoding
						) )
			;
		}

	}

}
