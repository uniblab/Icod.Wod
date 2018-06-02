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
		#endregion fieldsfs


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
		private System.Action<System.IO.StreamWriter> EolWriter {
			get {
				return myEolWriter ?? theEmptyEolWriter;
			}
		}
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}

		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.GetFileHandler( workOrder );
			if ( null == source ) {
				throw new System.InvalidOperationException();
			} else if ( 0 == this.Head ) {
				return;
			}

			var enc = this.GetEncoding();
			System.Int32 head = this.Head;
			System.Int32 tail = this.Tail;
			System.Int32 i;
			IQueue<System.String> bank;
			var rs = this.RecordSeparator;
			foreach ( var filePathName in source.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var s = source.OpenReader( filePathName ) ) {
						using ( var sr = new System.IO.StreamReader( s, enc ) ) {
							using ( var sw = new System.IO.StreamWriter( buffer, enc ) ) {
								i = head;
								while ( !sr.EndOfStream && ( 0 < i-- ) ) {
									sr.ReadLine( rs );
								}
								if ( 0 < tail ) {
									bank = Queue<System.String>.Empty;
									while ( !sr.EndOfStream && ( tail < bank.Count ) ) {
										bank = bank.Enqueue( sr.ReadLine( rs ) );
									}
									while ( !sr.EndOfStream ) {
										sw.Write( bank.Peek() );
										bank = bank.Dequeue().Enqueue( sr.ReadLine( rs ) );
									}
								} else {
									while ( !sr.EndOfStream ) {
										sw.Write( sr.ReadLine( rs ) );
									}
								}
							}
						}
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					source.Overwrite( buffer, filePathName );
				}
			}
		}
		#endregion methods

	}

}