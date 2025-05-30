// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"preambleFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class PreambleFile : BinaryFileOperationBase {

		#region .ctor
		public PreambleFile() : base() {
		}
		#endregion  .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"preamble",
			Namespace = "http://Icod.Wod"
		)]
		public System.String Preamble {
			get;
			set;
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var sourceHandler = this.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();
			var preamble = this.Preamble;
			if ( System.String.IsNullOrEmpty( preamble ) ) {
				throw new System.InvalidOperationException();
			}
			var dest = this.Destination ?? this;
			var destHandler = dest.GetFileHandler( workOrder );

			System.Action<System.IO.StreamWriter, System.IO.StreamReader, System.String> worker = this.PrefixOnce;
			var sourceEncoding = this.GetEncoding();
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var writer = new System.IO.StreamWriter( buffer, sourceEncoding, this.BufferLength, true ) ) {
						using ( var original = sourceHandler.OpenReader( file) ) {
							using ( var reader = new System.IO.StreamReader( original, this.GetEncoding(), true, this.BufferLength, true ) ) {
								worker( writer, reader, preamble );
							}
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					destHandler.Overwrite( buffer, dest.GetFilePathName( destHandler, file ) );
				}
			}
		}
		private void PrefixOnce( System.IO.StreamWriter destination, System.IO.StreamReader source, System.String preamble ) {
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
			destination = destination ?? throw new System.ArgumentNullException( nameof( destination ) );
			if ( System.String.IsNullOrEmpty( preamble ) ) {
				throw new System.ArgumentNullException( nameof( preamble ) );
			} else

			destination.Write( preamble );
			var rs = this.RecordSeparator;
			if ( !System.String.IsNullOrEmpty( rs ) ) {
				destination.Write( rs );
			}
			destination.Flush();
			source.BaseStream.CopyTo( destination.BaseStream );
		}
		#endregion methods

	}

}
