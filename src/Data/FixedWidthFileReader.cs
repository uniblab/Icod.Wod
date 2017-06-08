using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fixedWidthReader",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public class FixedWidthFileReader : FileBase {

		#region fields
		private System.String myRecordSeparator;
		#endregion fields


		#region .ctor
		public FixedWidthFileReader() : base() {
			myRecordSeparator = "\r\n";
		}
		public FixedWidthFileReader( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myRecordSeparator = "\r\n";
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"recordSeparator",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "\r\n" )]
		public System.String RecordSeparator {
			get {
				return myRecordSeparator;
			}
			set {
				myRecordSeparator = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source ) {
			throw new System.NotImplementedException();
		}

		protected sealed override System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			var output = new System.Data.DataTable();
			if ( !( this.Columns ?? new TextFileColumn[ 0 ] ).Any() ) {
				throw new System.InvalidOperationException();
			}
			System.Data.DataColumn col;
			System.Int32 l;
			foreach ( var c in this.Columns ) {
				col = new System.Data.DataColumn( c.Name );
				l = c.Length;
				if ( l <= 0 ) {
					throw new System.InvalidOperationException();
				}
				col.MaxLength = l;
				output.Columns.Add( col );
			}
			var fileNameColumn = new System.Data.DataColumn( "%wod:FileName%", typeof( System.String ) );
			fileNameColumn.AllowDBNull = false;
			fileNameColumn.ReadOnly = true;
			fileNameColumn.DefaultValue = fileName;
			output.Columns.Add( fileNameColumn );
			output.TableName = fileName;

			return output;
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
			foreach ( var c in cols ) {
				l = c.MaxLength;
				yield return record.Substring( i, l ).TrimToNull();
				i += l;
			}
		}
		#endregion methods

	}

}