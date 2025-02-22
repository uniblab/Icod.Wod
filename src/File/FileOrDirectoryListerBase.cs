// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( List ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ListDirectory ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ListFile ) )]
	public abstract class FileOrDirectoryListerBase : BinaryFileOperationBase, ITruncateEntryName {

		#region fields
		private System.Boolean myTruncateEntryName;
		#endregion fields


		#region .ctor
		protected FileOrDirectoryListerBase() : base() {
			myTruncateEntryName = true;
		}
		#endregion .ctor


		#region properties
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
		#endregion properties


		#region methods
		protected abstract System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source );
		public override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();

			System.Func<FileEntry, System.String> getFileName;
			if ( this.TruncateEntryName ) {
				getFileName = x => System.IO.Path.GetFileName( x.File );
			} else {
				getFileName = x => x.File;
			}

			var list = this.GetEntries( source );
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
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dh.Overwrite( buffer, dh.PathCombine( dest.ExpandedPath, dest.ExpandedName ) );
				}
			}
		}
		#endregion methods

	}

}
