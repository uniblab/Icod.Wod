// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"pruneFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class PruneFile : BinaryFileOperationBase {

		#region .ctor
		public PruneFile() : base() {
			this.TrimLines = true;
		}
		public PruneFile( WorkOrder workOrder ) : base( workOrder ) {
			this.TrimLines = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"trimLines",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public System.Boolean TrimLines {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceHandler = this.GetFileHandler( workOrder );
			if ( sourceHandler is null ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination;
			if ( dest is null ) {
				dest = this;
			}
			var destHandler = dest.GetFileHandler( workOrder );

			var sourceEncoding = CodePageHelper.GetCodePage( this.CodePage );
			var destEncoding = CodePageHelper.GetCodePage( this.CodePage );

			System.Func<System.String, System.String> trim = null;
			if ( this.TrimLines ) {
				trim = x => x.TrimToNull();
			} else {
				trim = x => x;
			}
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var source = sourceHandler.OpenReader( file ) ) {
						using ( var reader = new System.IO.StreamReader( source, sourceEncoding, true, this.BufferLength, true ) ) {
							using ( var writer = new System.IO.StreamWriter( buffer, destEncoding, this.BufferLength, true ) ) {
								var rs = this.RecordSeparator;
								System.String line = reader.ReadLine( rs );
								while ( null != line ) {
									line = trim( line );
									if ( !System.String.IsNullOrEmpty( line ) ) {
										writer.Write( line + rs );
									}
									line = reader.ReadLine( rs );
								}
							}
						}
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, file );
				}
			}
		}
		#endregion methods

	}

}
