using System.Linq;

namespace Icod.Wod {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class FileHelper {

		public static System.String ReadLine( this System.IO.StreamReader file, System.String recordSeparator, System.Char quoteChar ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				return null;
			} else if ( recordSeparator.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			} else {
				//throw new System.NotImplementedException();
				;
			}

			System.Boolean isReading = true;
			var rs = recordSeparator.ToCharArray();
			System.Int32 i = 0;
			var maxI = rs.Length;
			var line = new System.Text.StringBuilder();
			System.Int32 j = 0;
			System.Int32 c;
			System.Boolean isNull = true;
			System.Boolean isQuoted = false;
			System.Char probe;
			do {
				c = file.Read();
				if ( -1 == c ) {
					maxI = 0;
					isReading = false;
					break;
				}
				isNull = false;
				probe = System.Convert.ToChar( c );
				line.Append( probe );
				if ( quoteChar.Equals( probe ) ) {
					c = file.Peek();
					if ( -1 == c ) {
						maxI = 0;
						isReading = false;
						break;
					}
					probe = System.Convert.ToChar( c );
					if ( isQuoted ) {
						if ( !quoteChar.Equals( probe ) ) {
							isQuoted = false;
						}
					} else {
						isQuoted = true;
					}
				}
				if ( ( !isQuoted ) && ( line[ j ].Equals( rs[ i ] ) ) ) {
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
		public static System.String ReadLine( this System.IO.StreamReader file, System.String recordSeparator ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				return null;
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
					maxI = 0;
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

		public static System.String PathCombine( System.String path, System.Char pathSeparator, System.String name ) {
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			} else if ( System.String.IsNullOrEmpty( path ) ) {
				throw new System.ArgumentNullException( "path" );
			}
			var sep = pathSeparator.ToString();
			while ( path.EndsWith( sep ) ) {
				path = path.Substring( 0, path.Length - 1 );
			}
			while ( name.StartsWith( sep ) ) {
				name = name.Substring( 1 );
			}
			return path + sep + name;
		}

	}

}