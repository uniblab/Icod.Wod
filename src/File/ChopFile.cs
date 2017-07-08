using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"chopFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ChopFile : FileOperationBase {

		#region fields
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;

		private System.String myCodePage;
		private System.String myRecordSeparator;
		private System.Int32 myHead;
		private System.Int32 myTail;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		#endregion fields


		#region .ctor
		static ChopFile() {
			theEmptyEolWriter = x => {};
		}

		public ChopFile() : base() {
			myCodePage = "windows-1252";
			myRecordSeparator = "\r\n";
			myHead = 0;
			myTail = 0;
			myEolWriter = theEmptyEolWriter;
		}
		public ChopFile( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myRecordSeparator = "\r\n";
			myHead = 0;
			myTail = 0;
			myEolWriter = theEmptyEolWriter;
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
			"head",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Head {
			get {
				return myHead;
			}
			set {
				if ( value < 0 ) {
					throw new System.InvalidOperationException();
				}
				myHead = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"tail",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( 0 )]
		public System.Int32 Tail {
			get {
				return myTail;
			}
			set {
				if ( value < 0 ) {
					throw new System.InvalidOperationException();
				}
				myTail = value;
			}
		}

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
				myEolWriter = System.String.IsNullOrEmpty( value )
					? theEmptyEolWriter
					: x => x.Write( myRecordSeparator )
				;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		protected System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? theEmptyEolWriter;
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
			} else if ( 0 == this.Head ) {
				return;
			}

			var enc = this.GetEncoding();
			System.Collections.Generic.IList<System.String> lines;
			System.String line;
			System.Int32 count;
			var head = this.Head;
			System.Int32 tail;
			foreach ( var filePathName in source.ListFiles().Where(
				x => x.FileType.Equals( FileType.File )
			).Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var s = source.OpenReader( filePathName ) ) {
						using ( var sr = new System.IO.StreamReader( s, enc ) ) {
							for ( System.Int32 i = 0; i < head; i++ ) {
								sr.ReadLine( this.RecordSeparator );
							}
							lines = new System.Collections.Generic.List<System.String>();
							do {
								line = sr.ReadLine( this.RecordSeparator );
								if ( null != line ) {
									lines.Add( line );
								}
							} while ( null != line );
						}
					}
					count = lines.Count - 1;
					tail = count - this.Tail;
					for ( System.Int32 i = count; tail < i; i-- ) {
						lines.RemoveAt( i );
					}
					using ( var bw = new System.IO.StreamWriter( buffer, enc ) ) {
						count = lines.Count;
						for ( System.Int32 i = 0; i < count; i++ ) {
							bw.Write( lines[ i ] );
							this.EolWriter( bw );
						}
						lines.Clear();
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					source.Overwrite( buffer, filePathName );
				}
			}
		}
		#endregion methods

	}

}