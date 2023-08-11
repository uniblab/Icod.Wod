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
	public static class FileHelper {

		#region fields
		public const System.Int32 EOL = -1;
		public const System.Int32 EOF = EOL;
		#endregion fields


		#region methods
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

		public static System.String ReadLine( this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( ( 1 == recordSeparator.Length ) && recordSeparator.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			} else if ( EOF == file.Peek() ) {
				return null;
			}

			var output = new System.Text.StringBuilder();
			var leLen = recordSeparator.Length;
			var lenStop = leLen - 1;
			System.Int32 i = 0;
			System.Char c;
			System.Boolean isPlaintext = true;
			System.Int32 p = file.Read();
			System.Char q;
			while ( EOL != p ) {
				c = System.Convert.ToChar( p );
				output = output.Append( c );
				if ( isPlaintext ) {
					if ( quoteChar.Equals( c ) ) {
						isPlaintext = false;
					} else if ( ( leLen <= output.Length ) && ( recordSeparator[ i ].Equals( output[ ( output.Length - leLen ) - i ] ) ) ) {
						if ( lenStop <= ++i ) {
							_ = output.Remove( output.Length - leLen, leLen );
							break;
						}
					} else {
						i = 0;
					}
				} else {
					if ( quoteChar.Equals( c ) ) {
						p = file.Peek();
						if ( EOL == p ) {
							throw new System.IO.EndOfStreamException();
						}
						c = System.Convert.ToChar( p );
						if ( quoteChar.Equals( c ) ) {
							_ = file.Read();
							output = output.Append( c );
						} else {
							i = 0;
							isPlaintext = true;
						}
					}
				}
				p = file.Read();
			}

			return output.ToString();
		}
		public static System.String ReadLine( this System.IO.TextReader file, System.String recordSeparator ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( EOF == file.Peek() ) {
				return null;
			}

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
					break;
				}
				isNull = false;
				_ = line.Append( System.Convert.ToChar( c ) );
				if ( line[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				if ( i == maxI ) {
					break;
				}
				j++;
			} while ( true );
			_ = line.Remove( line.Length - maxI, maxI );
			return isNull ? null : line.ToString();
		}
		public static System.String ReadLine( this System.IO.StreamReader file, System.String recordSeparator, System.Char quoteChar ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( ( 1 == recordSeparator.Length ) && recordSeparator.Equals( quoteChar.ToString() ) ) {
				throw new System.InvalidOperationException( "Quote character and record separator cannot be the same." );
			} else if ( EOF == file.Peek() ) {
				return null;
			}

			var output = new System.Text.StringBuilder();
			var leLen = recordSeparator.Length;
			var lenStop = leLen - 1;
			System.Int32 i = 0;
			System.Char c;
			System.Boolean isPlaintext = true;
			System.Int32 p = file.Read();
			System.Char q;
			while ( EOL != p ) {
				c = System.Convert.ToChar( p );
				output = output.Append( c );
				if ( isPlaintext ) {
					if ( quoteChar.Equals( c ) ) {
						isPlaintext = false;
					} else if ( ( leLen <= output.Length ) && ( recordSeparator[ i ].Equals( output[ ( output.Length - leLen ) - i ] ) ) ) {
						if ( lenStop <= ++i ) {
							_ = output.Remove( output.Length - leLen, leLen );
							break;
						}
					} else {
						i = 0;
					}
				} else {
					if ( quoteChar.Equals( c ) ) {
						p = file.Peek();
						if ( EOL == p ) {
							throw new System.IO.EndOfStreamException();
						}
						c = System.Convert.ToChar( p );
						if ( quoteChar.Equals( c ) ) {
							_ = file.Read();
							output = output.Append( c );
						} else {
							i = 0;
							isPlaintext = true;
						}
					}
				}
				p = file.Read();
			}

			return output.ToString();
		}
		public static System.String ReadLine( this System.IO.StreamReader file, System.String recordSeparator ) {
			if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.ArgumentNullException( "recordSeparator" );
			} else if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( EOF == file.Peek() ) {
				return null;
			}

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
					break;
				}
				isNull = false;
				_ = line.Append( System.Convert.ToChar( c ) );
				if ( line[ j ].Equals( rs[ i ] ) ) {
					i++;
				} else {
					i = 0;
				}
				if ( i == maxI ) {
					break;
				}
				j++;
			} while ( true );
			_ = line.Remove( line.Length - maxI, maxI );
			return isNull ? null : line.ToString();
		}


		public static System.Collections.Generic.IEnumerable<System.String> ReadRecord( this System.IO.TextReader file, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.InvalidOperationException();
			}

			var line = file.ReadLine( recordSeparator, quoteChar );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String column;
				System.Nullable<System.Char> ec = null;
				var qc = quoteChar;
				do {
					i = reader.Peek();
					if ( -1 == i ) {
						break;
					}
					c = System.Convert.ToChar( i );
					if ( qc.Equals( c ) ) {
						_ = reader.Read();
						column = ReadColumn( reader, quoteChar, true );
						yield return column;
					} else {
						column = ReadColumn( reader, fieldSeparator, false );
						yield return column;
					}
				} while ( true );
			}
		}
		public static System.Collections.Generic.IEnumerable<System.String> ReadRecord( this System.IO.StreamReader file, System.String recordSeparator, System.Char quoteChar, System.Char fieldSeparator ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				yield break;
			} else if ( System.String.IsNullOrEmpty( recordSeparator ) ) {
				throw new System.InvalidOperationException();
			}

			var line = file.ReadLine( recordSeparator, quoteChar );
			if ( null == line ) {
				yield break;
			}
			using ( var reader = new System.IO.StringReader( line ) ) {
				System.Int32 i;
				System.Char c;
				System.String column;
				System.Nullable<System.Char> ec = null;
				var qc = quoteChar;
				do {
					i = reader.Peek();
					if ( -1 == i ) {
						break;
					}
					c = System.Convert.ToChar( i );
					if ( qc.Equals( c ) ) {
						_ = reader.Read();
						column = ReadColumn( reader, quoteChar, true );
						yield return column;
					} else {
						column = ReadColumn( reader, fieldSeparator, false );
						yield return column;
					}
				} while ( true );
			}
		}
		public static System.String ReadColumn( this System.IO.StringReader reader, System.Char @break, System.Boolean readNextOnBreak ) {
#if DEBUG
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif
			var sb = new System.Text.StringBuilder( 128 );
			System.Nullable<System.Char> ch;
			var reading = true;
			do {
				ch = ReadChar( reader, @break, readNextOnBreak );
				if ( ch.HasValue ) {
					sb = sb.Append( ch.Value );
				} else {
					reading = false;
					break;
				}
			} while ( reading );
			return sb.ToString().TrimToNull();
		}
		public static System.Nullable<System.Char> ReadChar( this System.IO.StringReader reader, System.Char @break, System.Boolean readNextOnBreak ) {
#if DEBUG
			if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif

			var p = reader.Peek();
			if ( -1 == p ) {
				return null;
			}
			var c = System.Convert.ToChar( reader.Read() );
			if ( @break.Equals( c ) ) {
				if ( readNextOnBreak ) {
					p = reader.Peek();
					if ( -1 == p ) {
						return null;
					} else if ( @break.Equals( System.Convert.ToChar( p ) ) ) {
						return System.Convert.ToChar( reader.Read() );
					} else {
						reader.Read();
					}
				}
				return null;
			}
			return c;
		}
		#endregion methods

	}

}
