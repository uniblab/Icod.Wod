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

	}

}