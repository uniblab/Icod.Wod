using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fixedWidthFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class FixedWidthFile : FileBase {

		#region .ctor
		public FixedWidthFile() : base() {
		}
		public FixedWidthFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( !( this.Columns ?? new TextFileColumn[ 0 ] ).Any() ) {
				throw new System.InvalidOperationException();
			} else if ( this.Columns.Any(
				x => ( 1 < x.Length )
			) ) {
				throw new System.ArgumentException( "All column lengths must be positive." );
			}
			return this.Columns.Select(
				x => new System.Data.DataColumn( x.Name, typeof( System.String ) ) {
					MaxLength = x.Length
				}
			);
		}
		protected sealed override System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( null == table ) {
				throw new System.ArgumentNullException( "table" );
			}
			return table.Rows.Add( this.ReadRecord( file ).ToArray() );
		}
		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( file.EndOfStream ) {
				yield break;
			}

			var cols = this.Columns.OfType<System.Data.DataColumn>().Where(
				x => x.ColumnName.StartsWith( "%wod:", System.StringComparison.OrdinalIgnoreCase ) && x.ColumnName.EndsWith( "%", System.StringComparison.OrdinalIgnoreCase )
			);
			var bufferLen = cols.Select(
				x => x.MaxLength
			).Sum() + ( this.RecordSeparator ?? System.String.Empty ).Length;
			var buffer = new System.Char[ bufferLen ];
			System.Int32 r = file.ReadBlock( buffer, 0, bufferLen );
			if ( 0 == r ) {
				yield break;
			}
			var record = new System.String( buffer, 0, r );
			System.Int32 l;
			System.Int32 i = 0;
			System.Func<System.String, System.Int32, System.Int32, System.String> w = ( a, b, c ) => a.Substring( b, c );
			var q = ( this.TrimValues )
				? ( a, b, c ) => w( a, b, c ).TrimToNull()
				: w
			;
			var getColValue = ( this.ConvertEmptyStringToNull )
				? ( a, b, c ) => q( a, b, c ) ?? System.String.Empty
				: q
			;
			foreach ( var c in cols ) {
				l = c.MaxLength;
				yield return getColValue( record, i, l );
				i += l;
			}
		}

		protected sealed override void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<TextFileColumn> fileColumns ) {
			if ( ( null == fileColumns ) || !fileColumns.Any() ) {
				throw new System.ArgumentNullException( "fileColumns" );
			} else if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( "dbColumns" );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			}

			var line = dbColumns.Select(
				x => x.ColumnName
			).Where(
				x => fileColumns.Select(
					y => y.Name
				).Contains( x, System.StringComparer.OrdinalIgnoreCase )
			).Select(
				x => x.PadRight( fileColumns.First(
					y => y.Name.Equals( x )
				).Length, ' ' ).Substring( 0, this.Columns.First(
					y => y.Name.Equals( x )
				).Length )
			);
			writer.Write( line );
			this.EolWriter( writer );
		}
		#endregion  methods

	}

}