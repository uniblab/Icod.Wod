// Copyright (C) 2025  Timothy J. Bruce

using System.Security.Cryptography;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class HttpFileHandler : RemoteFileHandlerBase {

		#region .ctor
		public HttpFileHandler() : base() {
		}
		public HttpFileHandler( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : base( workOrder, descriptor ) {
		}
		#endregion .ctor


		#region methods
		private void SetClient( System.Net.HttpWebRequest client, System.String method ) {
#if DEBUG
			client = client ?? throw new System.ArgumentNullException( nameof( client ) );
#endif
			var fd = this.FileDescriptor;
			var uri = new System.Uri( fd.ExpandedPath );
			var ub = new System.UriBuilder( fd.ExpandedPath );
			var username = uri.UserInfo.TrimToNull() ?? ub.UserName.TrimToNull() ?? fd.Username.TrimToNull();
			var passwd = ub.Password.TrimToNull() ?? fd.Password.TrimToNull();
			var host = uri.Host;
			client.Credentials = new System.Net.NetworkCredential(
				username,
				passwd
			);
			client.Method = method;
			client.AutomaticDecompression = System.Net.DecompressionMethods.None;
			client.PreAuthenticate = true;
			System.Net.ServicePointManager.SecurityProtocol = TlsHelper.GetSecurityProtocol();
		}

		public sealed override void TouchFile() {
			throw new System.NotSupportedException();
		}
		public sealed override void TruncateFile() {
			throw new System.NotSupportedException();
		}
		public sealed override void DeleteFile() {
			var fd = this.FileDescriptor;
			var filePathName = this.PathCombine( fd.ExpandedPath, fd.ExpandedName );
			this.DeleteFile( filePathName );
		}
		public sealed override void DeleteFile( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var http = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create( uri );
			this.SetClient( http, "DELETE" );
			using ( var response = (System.Net.HttpWebResponse)http.GetResponse() ) {
				switch ( response.StatusCode ) {
					case System.Net.HttpStatusCode.Accepted:
					case System.Net.HttpStatusCode.NoContent:
					case System.Net.HttpStatusCode.OK:
						break;
					default:
						throw new System.NotSupportedException();
				}
			}
		}

		public override System.IO.Stream OpenReader( System.String filePathName ) {
			var uri = new System.Uri( filePathName );
			var http = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create( uri );
			this.SetClient( http, System.Net.WebRequestMethods.Http.Get );
			var client = http.GetResponse();
			return new ClientStream( client.GetResponseStream(), client );
		}
		public sealed override void Overwrite( System.IO.Stream source, System.String filePathName ) {
			throw new System.NotSupportedException();
		}
		public sealed override void Append( System.IO.Stream source, System.String filePathName ) {
			throw new System.NotSupportedException();
		}
		public sealed override void MkDir() {
			throw new System.NotSupportedException();
		}
		public sealed override void RmDir( System.Boolean recurse ) {
			throw new System.NotSupportedException();
		}
		public sealed override void RmDir( System.String filePathName, System.Boolean recurse ) {
			throw new System.NotSupportedException();
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			return new FileEntry[ 1 ] {
				new FileEntry {
					FileType = FileType.Unknown,
					Handler = this,
					File = filePathName
				}
			};
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			return new FileEntry[ 1 ] {
				new FileEntry {
					FileType = FileType.Unknown,
					Handler = this,
					File = filePathName
				}
			};
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			var fd = this.FileDescriptor;
			var filePathName = System.String.IsNullOrEmpty( fd.ExpandedName )
				? fd.ExpandedPath
				: this.PathCombine( fd.ExpandedPath, fd.ExpandedName )
			;
			return new FileEntry[ 1 ] {
				new FileEntry {
					FileType = FileType.Unknown,
					Handler = this,
					File = filePathName
				}
			};
		}
#endregion methods

	}

}
