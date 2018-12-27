using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FtpFileHandler : RemoteFileHandlerBase {

		#region fields
		private static readonly System.Func<System.Byte[ ], System.String, System.Security.Cryptography.X509Certificates.X509Certificate> theStreamPasswdAuthMethodCtor;
		private static readonly System.Func<System.Byte[ ], System.String, System.Security.Cryptography.X509Certificates.X509Certificate> theStreamAuthMethodCtor;
		#endregion fields


		#region .ctor
		static FtpFileHandler() {
			theStreamPasswdAuthMethodCtor = ( data, passwd ) => new System.Security.Cryptography.X509Certificates.X509Certificate( data, passwd );
			theStreamAuthMethodCtor = ( data, passwd ) => new System.Security.Cryptography.X509Certificates.X509Certificate( data );
		}

		public FtpFileHandler() : base() {
		}
		public FtpFileHandler( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		public FtpFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}
		#endregion .ctor


		#region methods
		private System.Net.FtpWebRequest GetFtpClient( System.Uri uri, System.String method ) {
			var ub = new System.UriBuilder( uri );
			System.Net.FtpWebRequest client = null;

			if ( uri.Scheme.Equals( "ftps", System.StringComparison.OrdinalIgnoreCase ) ) {
				if ( uri.Port <= 0 ) {
					ub.Port = 990;
				}
				ub.Scheme = "ftp";
				client = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( ub.Uri );
				client.EnableSsl = true;
			} else {
				client = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( ub.Uri );
			}
			var fd = this.FileDescriptor;
			client.UsePassive = fd.UsePassive;
			var kf = fd.SshKeyFile;
			if ( null != kf ) {
				kf.WorkOrder = fd.WorkOrder;
				var kffd = kf.GetFileHandler( this.WorkOrder );
				var kfpasswd = kf.KeyFilePassword.TrimToNull();
				var action = System.String.IsNullOrEmpty( kfpasswd )
					? theStreamAuthMethodCtor
					: theStreamPasswdAuthMethodCtor
				;
				client.ClientCertificates.AddRange( kffd.ListFiles().Select(
					fe => {
						var kfs = new System.IO.MemoryStream();
						using ( var keySource = kffd.OpenReader( fe.File ) ) {
							keySource.CopyTo( kfs );
						}
						kfs.Seek( 0, System.IO.SeekOrigin.Begin );
						return action( kfs.ToArray(), kfpasswd );
					}
				).ToArray() );
			}
			var username = uri.UserInfo.TrimToNull() ?? ub.UserName.TrimToNull() ?? fd.Username.TrimToNull();
			var passwd = ub.Password.TrimToNull() ?? fd.Password.TrimToNull();
			client.Credentials = new System.Net.NetworkCredential(
				username,
				passwd
			);
			client.Method = method;
			//System.Net.ServicePointManager.ServerCertificateValidationCallback += ( System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors ) => ( System.Net.Security.SslPolicyErrors.None == sslPolicyErrors );
			System.Net.ServicePointManager.ServerCertificateValidationCallback += ( System.Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors ) => true;
			return client;
		}

		public sealed override void TouchFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.AppendFile );
			ftp.ContentLength = 0;
			using ( var dummy = ftp.GetRequestStream() ) {
				dummy.Flush();
			}
			ftp.GetResponse().Close();
		}
		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.DeleteFile );
			ftp.GetResponse().Close();
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.DeleteFile );
			ftp.GetResponse().Close();
		}

		public override System.IO.Stream OpenReader( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.DownloadFile );
			var client = ftp.GetResponse();
			return new ClientStream( client.GetResponseStream(), client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.UploadFile );
			this.Write( source, ftp );
			ftp.GetResponse().Close();
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.AppendFile );
			this.Write( source, ftp );
			ftp.GetResponse().Close();
		}

		private void Write( System.IO.Stream source, System.Net.FtpWebRequest client ) {
			using ( var buffer = client.GetRequestStream() ) {
				client.ContentLength = (System.Int32)this.Write( source, buffer );
			}
		}

		public sealed override void MkDir() {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( dirPath );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.MakeDirectory );
			ftp.GetResponse().Close();
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			var fd = this.FileDescriptor;
			System.String dirPath = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( dirPath );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.RemoveDirectory );
			ftp.GetResponse().Close();
		}
		public sealed override void RmDir( System.String filePathName, System.Boolean recurse ) {
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.RemoveDirectory );
			ftp.GetResponse().Close();
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.ListDirectoryDetails );
			var regexPattern = fd.WorkOrder.ExpandPseudoVariables( fd.RegexPattern );
			var list = this.ReadLines( ftp );
			return list.Select(
				x => new FileEntry {
					File = this.PathCombine( fd.ExpandedPath, this.StripNameFromList( x ) ),
					FileType = x.StartsWith( "d", System.StringComparison.OrdinalIgnoreCase )
						? FileType.Directory
						: FileType.File
					,
					Handler = this
				}
			).Where(
				x => System.String.IsNullOrEmpty( regexPattern )
					? true
					: System.Text.RegularExpressions.Regex.IsMatch( x.File, regexPattern )
			);
		}
		private System.String StripNameFromList( System.String listLine ) {
			var q = listLine.Split( new System.Char[ 1 ] { ' ' }, 9, System.StringSplitOptions.RemoveEmptyEntries );
			return q[ q.Length - 1 ];
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.ListDirectoryDetails );
			var ldd = this.ReadLines( ftp );
			var fileList = ldd.Where(
				x => !x.StartsWith( "d", System.StringComparison.OrdinalIgnoreCase )
			);
			var list = fileList.Select(
				x => {
					var y = x.Split( new System.Char[ 1 ] { ' ' }, 9, System.StringSplitOptions.RemoveEmptyEntries );
					return y[ y.Length - 1 ];
				}
			);
			var regexPattern = fd.WorkOrder.ExpandPseudoVariables( fd.RegexPattern );
			return ( System.String.IsNullOrEmpty( regexPattern )
				? list
				: list.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
				)
			).Where(
				x => !System.String.IsNullOrEmpty( System.IO.Path.GetExtension( x ) )
			).Select(
				x => new FileEntry {
					File = this.PathCombine( fd.ExpandedPath, x ),
					FileType = FileType.File,
					Handler = this
				}
			);
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			var uri = new System.Uri( filePathName );
			var ftp = this.GetFtpClient( uri, System.Net.WebRequestMethods.Ftp.ListDirectoryDetails );
			var regexPattern = fd.WorkOrder.ExpandPseudoVariables( fd.RegexPattern );
			var list = this.ReadLines( ftp ).Where(
				x => x.StartsWith( "d", System.StringComparison.OrdinalIgnoreCase )
			).Select(
				x => {
					var y = x.Split( new System.Char[ 1 ] { ' ' }, 9, System.StringSplitOptions.RemoveEmptyEntries );
					return y[ y.Length - 1 ];
				}
			);
			System.Func<System.String, System.Boolean> regexMatch = null;
			if ( System.String.IsNullOrEmpty( regexPattern ) ) {
				regexMatch = x => true;
			} else {
				regexMatch = x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern );
			}
			return list.Select(
				x => new FileEntry {
					File = this.PathCombine( fd.ExpandedPath, this.StripNameFromList( x ) ),
					FileType = x.StartsWith( "d", System.StringComparison.OrdinalIgnoreCase )
						? FileType.Directory
						: FileType.File
					,
					Handler = this
				}
			).Where(
				x => regexMatch( x.File )
			);
		}
		#endregion methods

	}

}