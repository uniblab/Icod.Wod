using System.Linq;

namespace Icod.Wod {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class FileHelper {

		public static System.String ReadLine( this System.IO.StreamReader file, System.String recordSeparator ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				return null;
			} else if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			}

			System.Boolean isReading = true;
			var rs = recordSeparator.ToCharArray();
			System.Int32 i = 0;
			var maxI = rs.Length;
			var line = new System.Text.StringBuilder();
			System.Int32 j = 0;
			System.Int32 c;
			System.Boolean isNull = true;
			do {
				c = file.Read();
				if ( -1 == c ) {
					isReading = false;
					break;
				}
				isNull = false;
				line.Append( System.Convert.ToChar( c ) );
				if ( line[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				if ( i == maxI ) {
					isReading = false;
					break;
				}
				j++;
			} while ( isReading );
			line.Remove( line.Length - maxI, maxI );
			return isNull ? null : line.ToString();
		}

	}

}