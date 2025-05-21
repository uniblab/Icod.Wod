// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"fixedWidthFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class FixedWidthFile : TextFileBase {

		#region fields
		private static readonly System.Func<System.String, System.Int32, System.Int32, System.String> theDefaultColumnReader;

		private System.Func<System.String, System.Int32, System.Int32, System.String> myColumnReader;
		#endregion fields


		#region .ctor
		static FixedWidthFile() {
			theDefaultColumnReader = ( a, b, c ) => a.Substring( b, c ).TrimToNull();
		}

		public FixedWidthFile() : base() {
			myColumnReader = theDefaultColumnReader;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"convertEmptyStringToNull",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public sealed override System.Boolean ConvertEmptyStringToNull {
			get {
				return base.ConvertEmptyStringToNull;
			}
			set {
				base.ConvertEmptyStringToNull = value;
				System.String w( System.String a, System.Int32 b, System.Int32 c ) => a.Substring( b, c );
				var q = ( this.TrimValues )
					? ( a, b, c ) => w( a, b, c ).TrimToNull()
					: (System.Func<System.String, System.Int32, System.Int32, System.String>)w
				;
				this.ColumnReader = ( value )
					? q
					: ( a, b, c ) => q( a, b, c ) ?? System.String.Empty
				;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"trimValues",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public sealed override System.Boolean TrimValues {
			get {
				return base.TrimValues;
			}
			set {
				base.TrimValues = value;
				System.String w( System.String a, System.Int32 b, System.Int32 c ) => a.Substring( b, c );
				var q = ( value )
					? ( a, b, c ) => w( a, b, c ).TrimToNull()
					: (System.Func<System.String, System.Int32, System.Int32, System.String>)w
				;
				this.ColumnReader = ( this.ConvertEmptyStringToNull )
					? q
					: ( a, b, c ) => q( a, b, c ) ?? System.String.Empty
				;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		private System.Func<System.String, System.Int32, System.Int32, System.String> ColumnReader {
			get {
				return myColumnReader ?? theDefaultColumnReader;
			}
			set {
				myColumnReader = value;
			}
		}
		#endregion properties


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StreamReader file ) {
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
			if ( !( this.Columns ?? new ColumnBase[ 0 ] ).Any() ) {
				throw new System.InvalidOperationException();
			} else if ( this.Columns.Any(
				x => ( x.Length < -1 )
			) ) {
				throw new System.InvalidOperationException( "All column lengths must be positive." );
			}
			return this.Columns.Select(
				x => new System.Data.DataColumn( x.Name, typeof( System.String ) ) {
					MaxLength = x.Length
				}
			);
		}
		protected sealed override System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file ) {
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
			table = table ?? throw new System.ArgumentNullException( nameof( table ) );
			return table.Rows.Add( this.ReadRecord( file ).ToArray() );
		}
		protected sealed override System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file ) {
			file = file ?? throw new System.ArgumentNullException( nameof( file ) );
			if ( file.EndOfStream ) {
				yield break;
			}

			var cols = this.Columns.OfType<System.Data.DataColumn>().Where(
				x => !x.ColumnName.Equals( "%wod:FileName%", System.StringComparison.OrdinalIgnoreCase )
			);
			var bufferLen = cols.Select(
				x => x.MaxLength
			).Sum() + ( this.RecordSeparator ?? System.String.Empty ).Length;
			var buffer = new System.Char[ bufferLen ];
			var r = file.ReadBlock( buffer, 0, bufferLen );
			if ( 0 == r ) {
				yield break;
			}
			var record = new System.String( buffer, 0, r );
			System.Int32 l;
			var i = 0;
			var colReader = this.ColumnReader;
			foreach ( var c in cols ) {
				l = c.MaxLength;
				yield return colReader( record, i, l );
				i += l;
			}
		}

		protected sealed override void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<ColumnBase> fileColumns ) {
			writer = writer ?? throw new System.ArgumentNullException( nameof( writer ) );
			if ( ( fileColumns is null ) || !fileColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( fileColumns ) );
			} else if ( ( dbColumns is null ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( dbColumns ) );
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
