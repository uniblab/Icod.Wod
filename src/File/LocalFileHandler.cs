// Copyright 2022, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class LocalFileHandler : FileHandlerBase {

		#region .ctor
		public LocalFileHandler() : base() {
		}
		public LocalFileHandler( Icod.Wod.WorkOrder workOrder ) : base( workOrder) {
		}
		public LocalFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) { 
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlIgnore]
		public sealed override System.Char PathSeparator {
			get {
				return System.IO.Path.DirectorySeparatorChar;
			}
		}
		#endregion properties


		#region methods
		public sealed override void TouchFile() {
			var filePathName = this.PathCombine( this.FileDescriptor.ExpandedPath, this.FileDescriptor.ExpandedName );
			using ( var file = System.IO.File.Open( filePathName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.Read | System.IO.FileShare.Write ) ) {
				System.IO.File.SetLastAccessTime( filePathName, System.DateTime.Now );
			}
		}
		public sealed override void DeleteFile() {
			foreach ( var fe in this.ListFiles() ) {
				this.DeleteFile( fe.File );
			}
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			System.IO.File.Delete( filePathName );
		}

		public sealed override System.IO.Stream OpenReader( System.String filePathName ) {
			return System.IO.File.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.OpenOrCreate );
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.Append );
		}
		private void Write( System.IO.Stream source, System.String filePathName, System.IO.FileMode fileMode ) {
			using ( var dest = System.IO.File.Open( filePathName, fileMode, System.IO.FileAccess.Write, System.IO.FileShare.Read ) ) {
				source.CopyTo( dest, this.BufferLength );
				dest.Flush();
				dest.SetLength( dest.Position );
			}
		}

		public sealed override void MkDir() {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			_ = System.IO.Directory.CreateDirectory( dirPath );
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			System.IO.Directory.Delete( dirPath, recurse );
		}
		public sealed override void RmDir( System.String filePathName, System.Boolean recurse ) {
			System.IO.Directory.Delete( filePathName, recurse );
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var regexPattern = fd.ExpandedRegexPattern;
			var list = System.IO.Directory.EnumerateFiles( fd.ExpandedPath, fd.ExpandedName, fd.SearchOption );
			return ( ( System.String.IsNullOrEmpty( regexPattern ) )
				? list
				: list.Where(
						x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
					)
			).Select(
				x => new FileEntry {
					File = x,
					FileType = FileType.File,
					Handler = this
				}
			);
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			var fd = this.FileDescriptor;
			var regexPattern = fd.ExpandedRegexPattern;
			var list = System.IO.Directory.EnumerateDirectories( fd.ExpandedPath, fd.ExpandedName, fd.SearchOption );
			return ( ( System.String.IsNullOrEmpty( regexPattern ) )
				? list
				: list.Where(
						x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
					)
			).Select(
				x => new FileEntry {
					File = x,
					FileType = FileType.Directory,
					Handler = this
				}
			);
		}
		#endregion methods

	}

}
