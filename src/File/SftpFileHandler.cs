using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public class SftpFileHandler : RemoteFileHandlerBase {

		#region .ctor
		public SftpFileHandler() : base() {
		}
		public SftpFileHandler( FileDescriptor descriptor ) : base( descriptor ) { 
		}
		#endregion .ctor


		#region methods
		private Renci.SshNet.SftpClient GetClient() {
			var fd = this.FileDescriptor;
			var passwd = fd.Password;
			var uri = new System.Uri( fd.ExpandedPath );
			var username = uri.UserInfo ?? fd.Username;
			var host = uri.Host;
			System.Int32 port = uri.Port;
			return ( -1 == port )
				? new Renci.SshNet.SftpClient( host, username, passwd )
				: new Renci.SshNet.SftpClient( host, port, username, passwd )
			;
		}
		public sealed override void TouchFile() {
			var fd = this.FileDescriptor;
			using ( var client = this.GetClient() ) {
				var file = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
				client.Open( file, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite ).Dispose();
			}
		}

		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			this.DeleteFile( new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath );
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			using ( var client = this.GetClient() ) {
				client.DeleteFile( filePathName );
			};
		}

		public sealed override System.IO.Stream OpenReader( System.String filePathName ) {
			var client = this.GetClient();
			return new ClientStream( client.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read ), client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.OpenOrCreate );
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.Append );
		}
		private void Write( System.IO.Stream source, System.String filePathName, System.IO.FileMode fileMode ) {
			using ( var client = this.GetClient() ) {
				using ( var dest = client.Open( filePathName, fileMode, System.IO.FileAccess.ReadWrite ) ) {
					this.Write( source, dest );
					dest.SetLength( dest.Position );
				}
			}
		}

		public sealed override void MkDir() {
			using ( var client = this.GetClient() ) {
				client.CreateDirectory( this.FileDescriptor.ExpandedPath );
			}
		}
		public sealed override void RmDir() {
			using ( var client = this.GetClient() ) {
				client.DeleteDirectory( this.FileDescriptor.ExpandedPath );
			}
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var pathName = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
			using ( var client = this.GetClient() ) {
				return this.GetRemoteList( client, pathName, fd.RegexPattern ).Where(
					x => x.IsRegularFile
				).Select(
					x => new FileEntry {
						File = x.FullName,
						Handler = this,
						FileType = FileType.File
					}
				);
			}
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			var fd = this.FileDescriptor;
			var pathName = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
			using ( var client = this.GetClient() ) {
				return this.GetRemoteList( client, pathName, fd.RegexPattern ).Where(
					x => x.IsDirectory
				).Select(
					x => new FileEntry {
						File = x.FullName,
						Handler = this,
						FileType = FileType.Directory
					}
				);
			}
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			var fd = this.FileDescriptor;
			var pathName = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
			using ( var client = this.GetClient() ) {
				return this.GetRemoteList( client, pathName, fd.RegexPattern ).Select(
					x => new FileEntry {
						File = x.FullName,
						Handler = this,
						FileType = x.IsDirectory
							? FileType.Directory
							: x.IsRegularFile
								? FileType.File
								: FileType.Unknown
					}
				);
			}
		}
		private System.Collections.Generic.IEnumerable<Renci.SshNet.Sftp.SftpFile> GetRemoteList( Renci.SshNet.SftpClient client, System.String filePathName, System.String regexPattern ) {
			if ( null == client ) {
				throw new System.ArgumentNullException( "client" );
			}
			var list = client.ListDirectory( filePathName );
			return System.String.IsNullOrEmpty( regexPattern )
				? list
				: list.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x.FullName, regexPattern )
				)
			;
		}
		#endregion methods

	}

}