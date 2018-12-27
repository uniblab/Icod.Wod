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
		public static System.String ExpandPseudoVariables( this System.String @string, WorkOrder workOrder, ContextRecord context ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return ( null == context )
				? @string.ExpandPseudoVariables( workOrder )
				: @string.ExpandPseudoVariables( workOrder, Stack<ContextRecord>.Empty.Push( context ) )
			;
		}
		public static System.String ExpandPseudoVariables( this System.String @string, WorkOrder workOrder, IStack<ContextRecord> context ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}

			System.Collections.Generic.IEnumerable<System.Data.DataColumn> cols;
			foreach ( var rec in ( context ?? Stack<ContextRecord>.Empty ).Where(
				x => ( null != x ) && ( null != x.Record )
			).Select(
				x => x.Record
			).Where(
				x => ( null != x ) && ( null != x.Table ) && ( null != x.Table.Columns )
			) ) {
				cols = rec.Table.Columns.OfType<System.Data.DataColumn>();
				if ( ( null == cols ) || !cols.Any() ) {
					continue;
				}
				foreach ( var col in cols ) {
					@string = @string.Replace( "%rec:" + col.ColumnName + "%", ( rec[ col ] ?? System.String.Empty ).ToString() );
				}
			}

			return @string.ExpandPseudoVariables( workOrder );
		}

	}

}