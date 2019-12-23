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

		public static System.String ExpandEnvironmentVariables( this System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return System.Environment.ExpandEnvironmentVariables( @string );
		}
		public static System.String ExpandPseudoVariables( this System.String @string, WorkOrder workOrder ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}

			if ( null != workOrder ) {
				@string = workOrder.ExpandPseudoVariables( @string );
			}
			@string = @string.ExpandEnvironmentVariables();
			return @string;
		}

		public static System.String Gunzip( System.Byte[] response, System.Text.Encoding encoding ) {
			using ( var input = new System.IO.MemoryStream( response, false ) ) {
				using ( var gunzip = new System.IO.Compression.GZipStream( input, System.IO.Compression.CompressionMode.Decompress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						gunzip.CopyTo( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						return encoding.GetString( buffer.ToArray() );
					}
				}
			}
		}
		public static System.Byte[] Gzip( System.String @string, System.Text.Encoding encoding ) {
			using ( var input = new System.IO.MemoryStream( encoding.GetBytes( @string ), false ) ) {
				using ( var gzip = new System.IO.Compression.GZipStream( input, System.IO.Compression.CompressionMode.Compress, true ) ) {
					using ( var buffer = new System.IO.MemoryStream() ) {
						gzip.CopyTo( buffer );
						buffer.Flush();
						buffer.Seek( 0, System.IO.SeekOrigin.Begin );
						return buffer.ToArray();
					}
				}
			}
		}

	}

}