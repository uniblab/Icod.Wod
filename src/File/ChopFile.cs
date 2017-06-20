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
		private System.Int32 myLines;
		private System.Action<System.IO.StreamWriter> myEolWriter;
		#endregion fields


		#region .ctor
		static ChopFile() {
			theEmptyEolWriter = x => {};
		}

		public ChopFile() : base() {
			myCodePage = "windows-1252";
			myRecordSeparator = "\r\n";
			myLines = 0;
			myEolWriter = theEmptyEolWriter;
		}
		public ChopFile( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myRecordSeparator = "\r\n";
			myLines = 0;
			myEolWriter = theEmptyEolWriter;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}

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
			"lines",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "\r\n" )]
		public System.Int32 Lines {
			get {
				return myLines;
			}
			set {
				myLines = value;
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
			var cp = this.CodePage;
			System.Int32 cpNumber;
			if ( System.Int32.TryParse( cp, out cpNumber ) ) {
				return System.Text.Encoding.GetEncoding( cpNumber );
			} else {
				return System.Text.Encoding.GetEncoding( cp );
			}
		}

		public sealed override void DoWork( WorkOrder order ) {
			if ( null == order ) {
				throw new System.ArgumentNullException( "order" );
			}
			this.WorkOrder = order;
			this.Destination.WorkOrder = order;
			var dest = this.Destination.GetFileHandler( order );
			var source = this.GetFileHandler( order );
			if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}

			var fd = dest.FileDescriptor;
			var sd = source.FileDescriptor;
			var enc = this.GetEncoding();
			System.Collections.Generic.IList<System.String> lines;
			System.String line;
			foreach ( var filePathName in source.ListFiles().Where(
				x => x.FileType.Equals( FileType.File )
			).Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream() ) {
					using ( var s = source.OpenReader( filePathName ) ) {
						using ( var sr = new System.IO.StreamReader( s, enc ) ) {
							lines = new System.Collections.Generic.List<System.String>();
							do {
								line = sr.ReadLine( this.RecordSeparator );
								if ( null != line ) {
									lines.Add( line );
								}
							} while ( null != line );
						}
					}
					using ( var bw = new System.IO.StreamWriter( buffer, enc ) ) {
						for ( System.Int32 i = this.Lines; i < 0; i++ ) {
							lines.RemoveAt( lines.Count - 1 );
						}
						for ( System.Int32 i = System.Math.Max( this.Lines, 0 ); i < lines.Count; i++ ) {
							bw.Write( lines[ i ] );
							this.EolWriter( bw );
						}
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dest.Overwrite( buffer, dest.PathCombine( fd.ExpandedPath, fd.ExpandedName ?? System.IO.Path.GetFileName( filePathName ) ) );
				}
			}
		}
		#endregion methods

	}

}