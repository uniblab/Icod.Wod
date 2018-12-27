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
		private const System.Int32 DefaultBufferSize = 16384;
		private static readonly System.Action<System.IO.StreamWriter> theEmptyEolWriter;

		private System.String myCodePage;
		private System.String myRecordSeparator;
		private System.Int32 myHead;
		private System.Int32 myTail;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		private System.Int32 myBufferSize;
		private System.Action<System.Int32, System.IO.StreamReader, System.String, System.IO.StreamWriter> tailWriter;
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

		[System.Xml.Serialization.XmlAttribute(
			"bufferSize",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultBufferSize )]
		public System.Int32 BufferSize {
			get {
				return myBufferSize;
			}
			set {
				if ( value <= 0 ) {
					throw new System.InvalidOperationException();
				}
				myBufferSize = value;
			}
		}
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}

		public sealed override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.GetFileHandler( workOrder );
			if ( null == source ) {
				throw new System.InvalidOperationException();
			}
			System.Int32 head = this.Head;
			System.Int32 tail = this.Tail;
			if ( ( 0 == head ) && ( 0 == tail ) ) {
				return;
			}

			if ( 0 == tail ) {
				tailWriter = StraightTailWriter;
			} else {
				tailWriter = BufferedTailWriter;
			}
			var enc = this.GetEncoding();
			var rs = this.RecordSeparator;
			foreach ( var filePathName in source.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( DefaultBufferSize ) ) {
					using ( var sw = new System.IO.StreamWriter( buffer, enc, this.BufferSize, true ) ) {
						using ( var s = source.OpenReader( filePathName ) ) {
							using ( var sr = new System.IO.StreamReader( s, enc, true, this.BufferSize, true ) ) {
								var i = head;
								while ( !sr.EndOfStream && ( 0 < i-- ) ) {
									sr.ReadLine( rs );
								}
								tailWriter( tail, sr, rs, sw );
							}
						}
						sw.Flush();
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					source.Overwrite( buffer, filePathName );
				}
			}
		}
		#endregion methods


		#region static methods
		private static void BufferedTailWriter( System.Int32 tail, System.IO.StreamReader reader, System.String recordSeparator, System.IO.StreamWriter writer ) {
#if DEBUG
			if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			} else if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif
			var bank = Queue<System.String>.Empty;
			while ( !reader.EndOfStream && ( bank.Count < tail ) ) {
				bank = bank.Enqueue( reader.ReadLine( recordSeparator ) );
			}
			while ( !reader.EndOfStream ) {
				writer.Write( bank.Peek() );
				bank = bank.Dequeue().Enqueue( reader.ReadLine( recordSeparator ) );
			}
		}
		private static void StraightTailWriter( System.Int32 tail, System.IO.StreamReader reader, System.String recordSeparator, System.IO.StreamWriter writer ) {
#if DEBUG
			if ( null == writer ) {
				throw new System.ArgumentNullException( "writer" );
			} else if ( null == reader ) {
				throw new System.ArgumentNullException( "reader" );
			}
#endif
			while ( !reader.EndOfStream ) {
				writer.Write( reader.ReadLine( recordSeparator ) );
			}
		}
		#endregion static methods

	}

}