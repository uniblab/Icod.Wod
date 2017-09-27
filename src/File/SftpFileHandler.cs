using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class SftpFileHandler : RemoteFileHandlerBase {

		#region .ctor
		public SftpFileHandler() : base() {
		}
		public SftpFileHandler( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		public SftpFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}
		#endregion .ctor


		#region methods
		private Renci.SshNet.SftpClient GetClient() {
			var fd = this.FileDescriptor;
			var uri = new System.Uri( fd.ExpandedPath );
			var ub = new System.UriBuilder( fd.ExpandedPath );
			var username = uri.UserInfo.TrimToNull() ?? ub.UserName.TrimToNull() ?? fd.Username.TrimToNull();
			var passwd = ub.Password.TrimToNull() ?? fd.Password.TrimToNull();
			var host = uri.Host;
			System.Int32 port = uri.Port;
			System.Collections.Generic.ICollection<Renci.SshNet.AuthenticationMethod> authMethods = new System.Collections.Generic.List<Renci.SshNet.AuthenticationMethod>( 2 );
			if ( !System.String.IsNullOrEmpty( passwd ) ) {
				authMethods.Add( new Renci.SshNet.PasswordAuthenticationMethod( username, passwd ) );
			}
			var kfd = fd.SshKeyFile;
			if ( null != kfd ) {
				kfd.WorkOrder = this.WorkOrder;
				var fp = kfd.Path.TrimToNull();
				var fn = kfd.Name.TrimToNull();
				var fpwd = kfd.Password.TrimToNull();
				if ( !System.String.IsNullOrEmpty( fp ) && !System.String.IsNullOrEmpty( fn ) ) {
					var kfpn = System.IO.Path.Combine( kfd.ExpandedPath, kfd.ExpandedName );
					var kfh = kfd.GetFileHandler( this.WorkOrder );
					var kfs = kfh.OpenReader( kfpn );
					authMethods.Add( new Renci.SshNet.PrivateKeyAuthenticationMethod(
						username,
						new Renci.SshNet.PrivateKeyFile[ 1 ] {
							System.String.IsNullOrEmpty( fpwd )
								? new Renci.SshNet.PrivateKeyFile( kfs )
								: new Renci.SshNet.PrivateKeyFile( kfs, fpwd )
						}
					) );
				}
			}
			var ama = authMethods.ToArray();
			var ci = ( -1 == port )
				? new Renci.SshNet.ConnectionInfo( host, username, ama )
				: new Renci.SshNet.ConnectionInfo( host, port, username, ama )
			;
			return new Renci.SshNet.SftpClient( ci );
		}
		public sealed override void TouchFile() {
			var fd = this.FileDescriptor;
			using ( var client = this.GetClient() ) {
				var file = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
				client.Connect();
				client.Open( file, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite ).Dispose();
			}
		}

		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			this.DeleteFile( new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath );
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			using ( var client = this.GetClient() ) {
				client.Connect();
				client.DeleteFile( filePathName );
			};
		}

		public sealed override System.IO.Stream OpenReader( System.String filePathName ) {
			var client = this.GetClient();
			client.Connect();
			return new ClientStream( client.Open( filePathName, System.IO.FileMode.Open, System.IO.FileAccess.Read ), client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var fpn = uri.LocalPath;
			this.Write( source, fpn, System.IO.FileMode.OpenOrCreate );
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.Append );
		}
		private void Write( System.IO.Stream source, System.String filePathName, System.IO.FileMode fileMode ) {
			using ( var client = this.GetClient() ) {
				client.Connect();
				using ( var dest = client.Open( filePathName, fileMode, System.IO.FileAccess.Write ) ) {
					source.CopyTo( dest, this.BufferLength );
					dest.Flush();
					dest.SetLength( dest.Position );
				}
			}
		}

		public sealed override void MkDir() {
			var dpn = new System.Uri( this.FileDescriptor.ExpandedPath ).AbsolutePath;
			using ( var client = this.GetClient() ) {
				client.Connect();
				client.CreateDirectory( dpn );
			}
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			using ( var client = this.GetClient() ) {
				client.Connect();
				client.DeleteDirectory( this.FileDescriptor.ExpandedPath );
			}
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var pathName = new System.Uri( this.PathCombine( fd.ExpandedPath, fd.ExpandedName ) ).AbsolutePath;
			using ( var client = this.GetClient() ) {
				client.Connect();
				return this.GetRemoteList( client, pathName, fd.WorkOrder.ExpandVariables( fd.RegexPattern ) ).Where(
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
				client.Connect();
				return this.GetRemoteList( client, pathName, fd.WorkOrder.ExpandVariables( fd.RegexPattern ) ).Where(
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
				client.Connect();
				return this.GetRemoteList( client, pathName, fd.WorkOrder.ExpandVariables( fd.RegexPattern ) ).Select(
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
			} else if ( !client.IsConnected ) {
				client.Connect();
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