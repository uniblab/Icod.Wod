using System.Linq;

namespace Icod.Wod {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public static class StringHelper {

		public static System.String TrimToNull( this System.String @string ) {
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			} else {
				@string = @string.Trim();
			}
			if ( System.String.IsNullOrEmpty( @string ) ) {
				@string = null;
			}
			return @string;
		}

	}

}