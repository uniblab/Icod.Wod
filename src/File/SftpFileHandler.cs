// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class SftpFileHandler : RemoteFileHandlerBase {

		#region fields
		private static readonly System.Func<System.IO.Stream, System.String, Renci.SshNet.PrivateKeyFile> theStreamPasswdAuthMethodCtor;
		private static readonly System.Func<System.IO.Stream, System.String, Renci.SshNet.PrivateKeyFile> theStreamAuthMethodCtor;
		#endregion fields


		#region .ctor
		static SftpFileHandler() {
			theStreamPasswdAuthMethodCtor = ( stream, passwd ) => new Renci.SshNet.PrivateKeyFile( stream, passwd );
			theStreamAuthMethodCtor = ( stream, passwd ) => new Renci.SshNet.PrivateKeyFile( stream );
		}

		public SftpFileHandler() : base() {
		}
		public SftpFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}

		#endregion .ctor


		#region methods
		private Renci.SshNet.SftpClient GetSftpClient( System.Uri uri ) {
			var ub = new System.UriBuilder( uri );

			var fd = this.FileDescriptor;
			var username = uri.UserInfo.TrimToNull() ?? ub.UserName.TrimToNull() ?? fd.Username.TrimToNull();
			var passwd = ub.Password.TrimToNull() ?? fd.Password.TrimToNull();
			var host = uri.Host;
			System.Int32 port = uri.Port;
			System.Collections.Generic.List<Renci.SshNet.AuthenticationMethod> ama = new System.Collections.Generic.List<Renci.SshNet.AuthenticationMethod>( 2 );
			var kf = fd.SshKeyFile;
			if ( null != kf ) {
				var wo = fd.WorkOrder;
				kf.WorkOrder = wo;
				var kffd = kf.GetFileHandler( wo );
				var kfpasswd = kf.KeyFilePassword.TrimToNull();
				var action = System.String.IsNullOrEmpty( kfpasswd )
					? theStreamAuthMethodCtor
					: theStreamPasswdAuthMethodCtor
				;
				ama.Add( new Renci.SshNet.PrivateKeyAuthenticationMethod( username, kffd.ListFiles().Select(
					fe => {
						var kfs = new System.IO.MemoryStream();
						using ( var keySource = kffd.OpenReader( fe.File ) ) {
							keySource.CopyTo( kfs );
						}
						_ = kfs.Seek( 0, System.IO.SeekOrigin.Begin );
						return kfs;
					}
				).Select(
					x => action( x, kfpasswd )
				).ToArray() ) );
			}
			ama.Add( new Renci.SshNet.PasswordAuthenticationMethod( username, passwd ) );
			var ci = ( -1 == port )
				? new Renci.SshNet.ConnectionInfo( host, username, ama.ToArray() )
				: new Renci.SshNet.ConnectionInfo( host, port, username, ama.ToArray() )
			;
			return new Renci.SshNet.SftpClient( ci );
		}

		public sealed override void TouchFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			using ( var client = this.GetSftpClient( uri ) ) {
				var file = uri.AbsolutePath;
				client.Connect();
				client.Open( file, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write ).Dispose();
			}
		}
		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			using ( var client = this.GetSftpClient( uri ) ) {
				var file = uri.AbsolutePath;
				client.Connect();
				client.DeleteFile( file );
			}
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			using ( var client = this.GetSftpClient( uri ) ) {
				var absolutePath = System.Uri.UnescapeDataString( uri.AbsolutePath );
				client.Connect();
				client.DeleteFile( absolutePath );
			}
		}

		public sealed override System.IO.Stream OpenReader( System.String filePathName ) {
			var ub = new System.UriBuilder( filePathName );
			var fd = this.FileDescriptor;
			if ( System.String.IsNullOrEmpty( ub.UserName ) ) {
				ub.UserName = fd.Username ?? new System.UriBuilder( fd.ExpandedPath ).UserName;
			}
			if ( System.String.IsNullOrEmpty( ub.Password ) ) {
				ub.Password = fd.Password ?? new System.UriBuilder( fd.ExpandedPath ).Password;
			}
			var uri = ub.Uri;
			Renci.SshNet.SftpClient client = null;
			System.IO.Stream stream = null;
			try {
				client = this.GetSftpClient( uri );
				client.Connect();
				var absolutePath = System.Uri.UnescapeDataString( uri.AbsolutePath );
				try {
					stream = client.Open( absolutePath, System.IO.FileMode.Open, System.IO.FileAccess.Read );
				} catch ( System.Exception ) {
					stream?.Dispose();
					throw;
				}
			} catch ( System.Exception ) {
				client?.Dispose();
				throw;
			}
			return new ClientStream( stream, client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.OpenOrCreate );
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			this.Write( source, filePathName, System.IO.FileMode.Append );
		}
		private void Write( System.IO.Stream source, System.String filePathName, System.IO.FileMode fileMode ) {
			var uri = new System.Uri( filePathName );
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				var absolutePath = System.Uri.UnescapeDataString( uri.AbsolutePath );
				using ( var dest = client.Open( absolutePath, fileMode, System.IO.FileAccess.Write ) ) {
					source.CopyTo( dest, this.BufferLength );
					dest.Flush();
					dest.SetLength( dest.Position );
				}
			}
		}

		public sealed override void MkDir() {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( dirPath );
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				client.CreateDirectory( uri.AbsolutePath );
			}
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( dirPath );
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				client.DeleteDirectory( uri.AbsolutePath );
			}
		}
		public sealed override void RmDir( System.String filePathName, System.Boolean recurse ) {
			var uri = new System.Uri( filePathName );
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				client.DeleteDirectory( uri.AbsolutePath );
			}
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var regexPattern = fd.ExpandedRegexPattern;
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				return GetRemoteList( client, uri.AbsolutePath, regexPattern ).Select(
					x => new FileEntry {
						File = BuildFullName( uri, x.FullName ),
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
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var regexPattern = fd.ExpandedRegexPattern;
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				return GetRemoteList( client, uri.AbsolutePath, regexPattern ).Where(
					x => x.IsRegularFile
				).Select(
					x => new FileEntry {
						File = BuildFullName( uri, x.FullName ),
						Handler = this,
						FileType = FileType.File
					}
				);
			}
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var regexPattern = fd.ExpandedRegexPattern;
			using ( var client = this.GetSftpClient( uri ) ) {
				client.Connect();
				return GetRemoteList( client, uri.AbsolutePath, regexPattern ).Where(
					x => x.IsDirectory
				).Select(
					x => new FileEntry {
						File = BuildFullName( uri, x.FullName ),
						Handler = this,
						FileType = FileType.Directory
					}
				);
			}
		}
		#endregion methods


		#region static methods
		private static System.String BuildFullName( System.Uri uri, System.String pathName ) {
			return new System.UriBuilder( "sftp", uri.Host, uri.Port, pathName ).Uri.ToString();
		}
		private static System.Collections.Generic.IEnumerable<Renci.SshNet.Sftp.ISftpFile> GetRemoteList( Renci.SshNet.SftpClient client, System.String filePathName, System.String regexPattern ) {
#if DEBUG
			if ( client is null ) {
				throw new System.ArgumentNullException( nameof( client ) );
			}
#endif
			if ( !client.IsConnected ) {
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
		#endregion

	}

}
