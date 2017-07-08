using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"listFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ListFile : BinaryFileOperationBase {

		#region fields
		private System.String myCodePage;
		private System.Boolean myTruncateEntryName;
		private System.Boolean myWriteIfEmpty;
		private System.Int32 myBufferLength;
		#endregion fields


		#region .ctor
		public ListFile() : base() {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
			myWriteIfEmpty = true;
			myBufferLength = 16384;
		}
		public ListFile( WorkOrder workOrder ) {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
			myWriteIfEmpty = true;
			myBufferLength = 16384;
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
			"truncateEntryName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean TruncateEntryName {
			get {
				return myTruncateEntryName;
			}
			set {
				myTruncateEntryName = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"writeIfEmpty",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean WriteIfEmpty {
			get {
				return myWriteIfEmpty;
			}
			set {
				myWriteIfEmpty = value;
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
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}

		public sealed override async void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.GetFileHandler( workOrder );
			if ( null == source ) {
				throw new System.InvalidOperationException();
			}

			System.Func<FileEntry, System.String> getFileName = null;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.File );
			} else {
				getFileName = x => x.File;
			}

			var list = source.ListFiles();
			if ( this.WriteIfEmpty || list.Any() ) {
				var dest = this.Destination;
				dest.WorkOrder = workOrder;
				var dh = dest.GetFileHandler( workOrder );
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var writer = new System.IO.StreamWriter( buffer, this.GetEncoding(), this.BufferLength, true ) ) {
						foreach ( var entry in list ) {
							writer.WriteLine( getFileName( entry ) );
						}
						writer.Flush();
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dh.Overwrite( buffer, dh.PathCombine( dest.ExpandedPath, dest.ExpandedName ) );
				}
			}
		}
		#endregion methods

	}

}