using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( DelimitedFileReader ) )]
	[System.Xml.Serialization.XmlInclude( typeof( DelimitedFileWriter ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FixedWidthFileReader ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FixedWidthFileWriter ) )]
	public abstract class FileBase : Icod.Wod.File.FileDescriptor, ITableDestination, ITableSource {

		#region fields
		private System.String myCodePage;
		private System.Boolean myHasHeader;
		private TextFileColumn[] myColumns;
		private System.Int32 myBufferLength;
		private System.Boolean myAppend;
		private System.Int32 mySkip;
		#endregion fields


		#region .ctor
		protected FileBase() : base() {
			myCodePage = "windows-1252";
			myHasHeader = true;
			myColumns = null;
			myBufferLength = 16384;
			myAppend = false;
			mySkip = 0;
		}
		protected FileBase( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myHasHeader = true;
			myColumns = null;
			myBufferLength = 16384;
			myAppend = false;
			mySkip = 0;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "windows-1252" )]
		public System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}

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

		[System.Xml.Serialization.XmlArray(
			"columns",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"column",
			typeof( TextFileColumn ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual TextFileColumn[] Columns {
			get {
				return myColumns;
			}
			set {
				myColumns = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"bufferLength",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 16384 )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"append",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Append {
			get {
				return myAppend;
			}
			set {
				myAppend = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"skip",
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
		#endregion properties


		#region methods
		public virtual System.Text.Encoding GetEncoding() {
			var cp = this.CodePage;
			System.Int32 cpNumber;
			if ( System.Int32.TryParse( cp, out cpNumber ) ) {
				return System.Text.Encoding.GetEncoding( cpNumber );
			} else {
				return System.Text.Encoding.GetEncoding( cp );
			}
		}

		public abstract void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source );

		public virtual System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder order ) {
			this.WorkOrder = order;
			System.String fileName;
			foreach ( var file in this.GetFiles() ) {
				fileName = System.IO.Path.GetFileName( file.File );
				using ( var stream = this.OpenReader( file ) ) {
					yield return this.ReadFile( fileName, stream );
				}
			}
		}

		protected virtual System.Collections.Generic.IEnumerable<Icod.Wod.File.FileEntry> GetFiles() {
			return this.GetFileHandler( this.WorkOrder ).ListFiles();
		}
		protected virtual System.IO.StreamReader OpenReader( Icod.Wod.File.FileEntry file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( Icod.Wod.File.FileType.Directory == file.FileType ) {
				throw new System.InvalidOperationException();
			}
			return new System.IO.StreamReader( this.GetFileHandler( this.WorkOrder ).OpenReader( file.File ), this.GetEncoding(), true, this.BufferLength );
		}

		protected abstract System.Data.DataTable BuildTable( System.String fileName, System.IO.StreamReader file );
		protected abstract System.Data.DataRow ReadRecord( System.Data.DataTable table, System.IO.StreamReader file );
		protected virtual System.Data.DataTable ReadFile( System.String fileName, System.IO.StreamReader file ) {
			if ( null == file ) {
				throw new System.ArgumentNullException( "file" );
			} else if ( System.String.IsNullOrEmpty( fileName ) ) {
				throw new System.ArgumentNullException( "fileName" );
			}

			var table = this.BuildTable( fileName, file );
			if ( !file.EndOfStream ) {
				System.Int32 s = 0;
				var skip = this.Skip;
				while ( ( ++s < skip ) && !file.EndOfStream ) {
					this.ReadRecord( file );
				}
				while ( !file.EndOfStream ) {
					this.ReadRecord( table, file );
				}
			}
			return table;
		}

		protected abstract System.Collections.Generic.IEnumerable<System.String> ReadRecord( System.IO.StreamReader file );
		#endregion methods

	}

}