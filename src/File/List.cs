// Copyright 2022, Timothy J. Bruce
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
		public List( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		protected sealed override System.Collections.Generic.IEnumerable<FileEntry> GetEntries( FileHandlerBase source ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}
			return source.List();
		}
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var source = this.GetFileHandler( workOrder );
			if ( null == source ) {
				throw new System.InvalidOperationException();
			}

			System.Func<FileEntry, System.String> getFileName = null;
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
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					dh.Overwrite( buffer, dh.PathCombine( dest.ExpandedPath, dest.ExpandedName ) );
				}
			}
		}
	}
	#endregion methods

}
