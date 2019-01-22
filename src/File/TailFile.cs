using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"tailFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class TailFile : CountBinaryFileOperationBase {

		#region .ctor
		public TailFile() : base() {
		}
		public TailFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceHandler = this.GetFileHandler( workOrder );
			if ( null == sourceHandler ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination;
			if ( null == dest ) {
				throw new System.InvalidOperationException( "Destination cannot be null." );
			}
			var destHandler = dest.GetFileHandler( workOrder );

			System.Func<FileHandlerBase, System.String, System.Text.Encoding, IQueue<System.String>> reader = null;
			var count = this.Count;
			if ( 0 == count ) {
				throw new System.InvalidOperationException( "Count may not be 0." );
			} else if ( 0 < count ) {
				reader = this.ReadPositiveCount;
			} else {
				reader = this.ReadNegativeCount;
			}
			var sourceEncoding = this.GetEncoding();
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var writer = new System.IO.StreamWriter( buffer, sourceEncoding ) ) {
						var rs = this.RecordSeparator;
						foreach ( var line in reader( sourceHandler, file, sourceEncoding ) ) {
							writer.Write( line + rs );
						}
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, dest.GetFilePathName( destHandler, file ) );
				}
			}
		}
		protected sealed override IQueue<System.String> ReadPositiveCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Queue<System.String>.Empty;
			System.String line = null;
			System.Int32 lineCount = 0;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					line = reader.ReadLine( rs );
					while ( null != line ) {
						output = output.Enqueue( line );
						line = reader.ReadLine( rs );
					}
				}
			}
			var count = this.Count;
			for ( var i = 0; i < count; i++ ) {
				if ( output.IsEmpty ) {
					break;
				}
				output = output.Dequeue();
			}
			return output;
		}
		protected sealed override IQueue<System.String> ReadNegativeCount( FileHandlerBase fileHandler, System.String filePathName, System.Text.Encoding encoding ) {
			var output = Queue<System.String>.Empty;
			System.String line = null;
			using ( var stream = fileHandler.OpenReader( filePathName ) ) {
				using ( var reader = new System.IO.StreamReader( stream, encoding, true, fileHandler.BufferLength ) ) {
					var rs = this.RecordSeparator;
					var count = -this.Count;
					for ( var i = 0; i < count; i++ ) {
						line = reader.ReadLine( rs );
						if ( null == line ) {
							break;
						}
					}
					line = reader.ReadLine();
					while ( null != line ) {
						output = output.Enqueue( line );
						line = reader.ReadLine( rs );
					}
				}
			}
			return output;
		}
		#endregion methods

	}

}