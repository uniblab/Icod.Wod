using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FtpFileHandler : RemoteFileHandlerBase {

		#region .ctor
		public FtpFileHandler() : base() {
		}
		public FtpFileHandler( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
		}
		public FtpFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}
		#endregion .ctor


		#region methods
		private void SetFtpClient( System.Net.FtpWebRequest client, System.String method ) {
			if ( null == client ) {
				throw new System.ArgumentNullException( "client" );
			}
			var fd = this.FileDescriptor;
			var uri = new System.Uri( fd.ExpandedPath );
			var ub = new System.UriBuilder( fd.ExpandedPath );
			var username = uri.UserInfo.TrimToNull() ?? ub.UserName.TrimToNull() ?? fd.Username.TrimToNull();
			var passwd = ub.Password.TrimToNull() ?? fd.Password.TrimToNull();
			var host = uri.Host;
			System.Int32 port = uri.Port;
			var credential = (System.Net.NetworkCredential)client.Credentials;
			client.Credentials = new System.Net.NetworkCredential(
				username,
				passwd
			);
			client.UsePassive = fd.UsePassive;
			client.Method = method;
		}

		public sealed override void TouchFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.AppendFile );
			ftp.ContentLength = 0;
			using ( var dummy = ftp.GetRequestStream() ) {
				dummy.Flush();
			}
			ftp.GetResponse().Close();
		}
		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			this.DeleteFile( filePathName );
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.DeleteFile );
			ftp.GetResponse().Close();
		}

		public override System.IO.Stream OpenReader( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.DownloadFile );
			var client = ftp.GetResponse();
			return new ClientStream( client.GetResponseStream(), client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.UploadFile );
			this.Write( source, ftp );
			ftp.GetResponse().Close();
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.AppendFile );
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
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.MakeDirectory );
			ftp.GetResponse().Close();
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.RemoveDirectory );
			ftp.GetResponse().Close();
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.ListDirectory );
			var regexPattern = fd.RegexPattern;
			var list = this.ReadLines( ftp );
			return ( System.String.IsNullOrEmpty( regexPattern )
				? list
				: list.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
				)
			).Select(
				x => new FileEntry {
					File = x,
					FileType = System.String.IsNullOrEmpty( System.IO.Path.GetExtension( x ) )
						? FileType.Directory
						: FileType.File
					,
					Handler = this
				}
			);
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.ListDirectory );
			var regexPattern = fd.RegexPattern;
			var list = this.ReadLines( ftp );
			return ( System.String.IsNullOrEmpty( regexPattern )
				? list
				: list.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
				)
			).Where(
				x => !System.String.IsNullOrEmpty( System.IO.Path.GetExtension( x ) )
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
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			var uri = new System.Uri( filePathName );
			var ftp = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create( uri );
			this.SetFtpClient( ftp, System.Net.WebRequestMethods.Ftp.ListDirectory );
			var regexPattern = fd.RegexPattern;
			var list = this.ReadLines( ftp );
			return ( System.String.IsNullOrEmpty( regexPattern )
				? list
				: list.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x, regexPattern )
				)
			).Where(
				x => System.String.IsNullOrEmpty( System.IO.Path.GetExtension( x ) )
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