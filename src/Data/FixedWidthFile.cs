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
		public FixedWidthFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
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
				System.Func<System.String, System.Int32, System.Int32, System.String> w = ( a, b, c ) => a.Substring( b, c );
				var q = ( this.TrimValues )
					? ( a, b, c ) => w( a, b, c ).TrimToNull()
					: w
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
				System.Func<System.String, System.Int32, System.Int32, System.String> w = ( a, b, c ) => a.Substring( b, c );
				var q = ( value )
					? ( a, b, c ) => w( a, b, c ).TrimToNull()
					: w
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
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( !( this.Columns ?? new ColumnBase[ 0 ] ).Any() ) {
				throw new System.InvalidOperationException();
			} else if ( this.Columns.Any(
				x => ( x.Length < -1 )
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
				throw new System.ArgumentNullException( nameof( file ) );
			} else if ( null == table ) {
				throw new System.ArgumentNullException( nameof( table ) );
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
			if ( ( fileColumns is null ) || !fileColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( fileColumns ) );
			} else if ( ( null == dbColumns ) || !dbColumns.Any() ) {
				throw new System.ArgumentNullException( nameof( dbColumns ) );
			} else if ( null == writer ) {
				throw new System.ArgumentNullException( nameof( writer ) );
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
