// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"list",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class List : FileOrDirectoryListerBase {

		#region .ctor
		public List() : base() {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			if ( source is null ) {
				throw new System.ArgumentNullException( nameof( source ) );
			}
			return source.List();
		}
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
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
							writer.WriteLine( "\"{0}\",\"{1}\"", entry.FileType, getFileName( entry ) );
						}
						writer.Flush();
					}
					_ = buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dh.Overwrite( buffer, dh.PathCombine( dest.ExpandedPath, dest.ExpandedName ) );
				}
			}
		}
	}
	#endregion methods

}
