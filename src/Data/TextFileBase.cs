// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DelimitedFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FixedWidthFile ) )]
	public abstract class TextFileBase : DataFileBase, IRecordSeparator {

		#region fields
		protected static readonly System.Action<System.IO.StreamWriter> EmptyEolWriter;

		private System.Action<System.IO.StreamWriter> myEolWriter;
		private System.Boolean myHasHeader;
		private System.Int32 mySkip;
		private System.Boolean myConvertEmptyStringToNull;
		private System.Boolean myTrimValues;
		#endregion fields


		#region .ctor
		static TextFileBase() {
			EmptyEolWriter = x => {
			};
		}

		protected TextFileBase() : base() {
			myHasHeader = true;
			mySkip = 0;
			myConvertEmptyStringToNull = true;
			myTrimValues = true;
			myEolWriter = x => x.Write( DefaultRecordSeparator );
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"hasHeader",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean HasHeader {
			get {
				return myHasHeader;
			}
			set {
				myHasHeader = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"skipCount",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Skip {
			get {
				return mySkip;
			}
			set {
				if ( value < 0 ) {
					throw new System.ArgumentOutOfRangeException( "value", "Parameter cannot be negative." );
				}
				mySkip = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		protected System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? EmptyEolWriter;
			}
			set {
				myEolWriter = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"convertEmptyStringToNull",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public virtual System.Boolean ConvertEmptyStringToNull {
			get {
				return myConvertEmptyStringToNull;
			}
			set {
				myConvertEmptyStringToNull = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"trimValues",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public virtual System.Boolean TrimValues {
			get {
				return myTrimValues;
			}
			set {
				myTrimValues = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"nullReplacementText",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual System.String NullReplacementText {
			get;
			set;
		}
		#endregion properties


		#region methods
		protected virtual System.String GetRow( System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( row is null ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( columns is null ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( formatMap is null ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			return System.String.Join( System.String.Empty, columns.Select(
				x => this.GetColumn( formatMap[ x ], x, row )
			) );
		}
		protected System.Collections.Generic.IEnumerable<System.String> GetColumns( System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( row is null ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( columns is null ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( formatMap is null ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			}

			return columns.Select(
				x => this.GetColumn( formatMap[ x ], x, row )
			);
		}
		protected System.String GetColumn( ColumnBase format, System.Data.DataColumn column, System.Data.DataRow row ) {
			if ( row is null ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( column is null ) {
				throw new System.ArgumentNullException( "column" );
			}

			if ( format is null ) {
				format = new TextFileColumn(){ Name = column.ColumnName };
			}
			var output = format.GetColumnText( this.WorkOrder, row[ column ] ) ?? this.NullReplacementText ?? System.String.Empty;
			if ( 0 < format.Length ) {
				var l = format.Length;
				var w = l - output.Length;
				if ( 0 < w ) {
					output = output.PadLeft( w, SpaceChar );
				} else if ( w < 0 ) {
					output = output.Substring( 0, l );
				}
			}
			return output;
		}

		protected sealed override System.Data.DataTable ReadFile( System.String filePathName, System.IO.StreamReader file ) {
			if ( file is null ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( filePathName ) ) {
				throw new System.ArgumentNullException( "filePathName" );
			}

			var table = new System.Data.DataTable();
			try {
				this.BuildTable( file, table );
				if ( !file.EndOfStream ) {
					this.ReadPreamble( file );
					while ( !file.EndOfStream ) {
						_ = this.ReadRecord( table, file );
					}
				}
				this.AddFileColumns( table, filePathName );
			} catch ( System.Exception e ) {
				if ( !e.Data.Contains( "%wod:FilePathName%" ) ) {
					e.Data.Add( "%wod:FilePathName%", filePathName );
				} else {
					e.Data[ "%wod:FilePathName%" ] = filePathName;
				}
				if ( !e.Data.Contains( "%wod:FileName%" ) ) {
					e.Data.Add( "%wod:FileName%", System.IO.Path.GetFileName( filePathName ) );
				} else {
					e.Data[ "%wod:FileName%" ] = System.IO.Path.GetFileName( filePathName );
				}
				throw;
			}
			return table;
		}
		private void BuildTable( System.IO.StreamReader file, System.Data.DataTable table ) {
#if DEBUG
			if ( table is null ) {
				throw new System.ArgumentNullException( "table" );
			} else if ( file is null ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif

			foreach ( var column in this.BuildColumns( file ) ) {
				table.Columns.Add( column );
			}
		}

		protected sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Collections.Generic.IEnumerable<System.Data.DataRow> rows ) {
#if DEBUG
			if ( workOrder is null ) {
				throw new System.ArgumentNullException( "workOrder" );
			}
#endif
			var cols = ( columns ?? new System.Data.DataColumn[ 0 ] );
			if ( this.WriteIfEmpty ) {
				if ( !cols.Any() ) {
					throw new System.ArgumentNullException( "columns" );
				}
			} else if (
				( !( rows ?? new System.Data.DataRow[ 0 ] ).Any() )
				|| ( !cols.Any() )
			) {
				return;
			}
			using ( var buffer = new System.IO.MemoryStream() ) {
				using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength ) ) {
					this.WriteHeader( writer, columns );
					var formatMap = this.BuildFormatMap( columns );
					foreach ( var row in rows ) {
						this.WriteRow( writer, formatMap, columns, row );
					}
					writer.Flush();
					this.WriteFile( buffer );
				}
			}
		}
		protected virtual void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> sourceColumns ) {
			if ( this.HasHeader ) {
				this.WriteHeader( writer, sourceColumns, this.Columns );
			}
		}
		protected abstract void WriteHeader( System.IO.StreamWriter writer, System.Collections.Generic.IEnumerable<System.Data.DataColumn> dbColumns, System.Collections.Generic.IEnumerable<ColumnBase> fileColumns );
		protected void WriteRow( System.IO.StreamWriter writer, System.Collections.Generic.IDictionary<System.Data.DataColumn, ColumnBase> formatMap, System.Collections.Generic.IEnumerable<System.Data.DataColumn> columns, System.Data.DataRow row ) {
			if ( row is null ) {
				throw new System.ArgumentNullException( "row" );
			} else if ( ( columns is null ) || !columns.Any() ) {
				throw new System.ArgumentNullException( "columns" );
			} else if ( ( formatMap is null ) || !formatMap.Any() ) {
				throw new System.ArgumentNullException( "formatMap" );
			} else if ( writer is null ) {
				throw new System.ArgumentNullException( "writer" );
			}
			writer.Write( this.GetRow( formatMap, columns, row ) );
			this.EolWriter( writer );
		}

		private void ReadPreamble( System.IO.StreamReader file ) {
#if DEBUG
			if ( file is null ) {
				throw new System.ArgumentNullException( "file" );
			}
#endif
			var bs = file.BaseStream;
			if ( ( bs is null ) || !bs.CanRead ) {
				throw new System.InvalidOperationException();
			}
			var skip = this.Skip;
			var s = 0;
			while ( !file.EndOfStream && ( s < skip ) ) {
				this.ReadRecord( file );
				s++;
			}
		}
		protected abstract System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file );
		protected abstract System.Collections.Generic.IEnumerable<System.Data.DataColumn> BuildColumns( System.IO.StreamReader file );
		protected abstract System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file );
		#endregion methods

	}

}
