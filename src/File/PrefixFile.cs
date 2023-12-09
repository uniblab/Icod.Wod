// Copyright (C) 2023  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"prefixFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class PrefixFile : BinaryFileOperationBase {

		#region .ctor
		public PrefixFile() : base() {
		}
		#endregion  .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"prefix",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Prefix {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceHandler = this.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();
			var prefix = this.Prefix;
			if ( System.String.IsNullOrEmpty( prefix ) ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination ?? this;
			var destHandler = dest.GetFileHandler( workOrder );

			var rs = this.RecordSeparator;
			System.Action<System.IO.StreamWriter, System.IO.StreamReader, System.String> worker = null;
			if ( System.String.IsNullOrEmpty( rs ) ) {
				worker = this.PrefixOnce;
			} else {
				worker = this.PrefixEach;
			}
			var sourceEncoding = this.GetEncoding();
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var writer = new System.IO.StreamWriter( buffer, sourceEncoding, this.BufferLength, true ) ) {
						using ( var original = sourceHandler.OpenReader( file) ) {
							using ( var reader = new System.IO.StreamReader( original, this.GetEncoding(), true, this.BufferLength, true ) ) {
								worker( writer, reader, prefix );
							}
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, dest.GetFilePathName( destHandler, file ) );
				}
			}
		}
		private void PrefixOnce( System.IO.StreamWriter destination, System.IO.StreamReader source, System.String prefix ) {
			if ( System.String.IsNullOrEmpty( prefix ) ) {
				throw new System.ArgumentException( "prefix" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( destination is null ) {
				throw new System.ArgumentNullException( "destination" );
			}
			destination.Write( prefix );
			destination.Flush();
			source.BaseStream.CopyTo( destination.BaseStream );
		}
		private void PrefixEach( System.IO.StreamWriter destination, System.IO.StreamReader source, System.String prefix ) {
			if ( System.String.IsNullOrEmpty( prefix ) ) {
				throw new System.ArgumentException( "prefix" );
			} else if ( source is null ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( destination is null ) {
				throw new System.ArgumentNullException( "destination" );
			}

			var rs = this.RecordSeparator;
			System.String line;
			while ( !source.EndOfStream ) {
				line = source.ReadLine( rs );
				if ( !System.String.IsNullOrEmpty( line ) ) {
					destination.Write( prefix );
					destination.Write( line );
					destination.Write( rs );
				}
			}
		}
		#endregion methods

	}

}
