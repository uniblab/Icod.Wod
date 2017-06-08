using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public class HttpFileHandler : RemoteFileHandlerBase {

		#region .ctor
		public HttpFileHandler() : base() {
		}
		public HttpFileHandler( FileDescriptor descriptor ) : base( descriptor ) {
		}
		#endregion .ctor


		#region methods
		protected void SetClient( System.Net.HttpWebRequest client, System.String method ) {
			if ( null == client ) {
				throw new System.ArgumentNullException( "client" );
			}
			var fd = this.FileDescriptor;
			var credential = (System.Net.NetworkCredential)client.Credentials;
			client.Credentials = new System.Net.NetworkCredential(
				fd.Username.TrimToNull() ?? credential.UserName,
				fd.Password.TrimToNull() ?? credential.Password
			);
			client.Method = method;
		}

		public sealed override void TouchFile() {
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
					case System.Net.HttpStatusCode.Accepted :
					case System.Net.HttpStatusCode.NoContent :
					case System.Net.HttpStatusCode.OK :
						break;
					default :
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
		public sealed override void RmDir() {
			throw new System.NotSupportedException();
		}

		public sealed override System.Collections.Generic.IEnumerable<FileEntry> List() {
			throw new System.NotSupportedException();
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListFiles() {
			throw new System.NotSupportedException();
		}
		public sealed override System.Collections.Generic.IEnumerable<FileEntry> ListDirectories() {
			throw new System.NotSupportedException();
		}
		#endregion methods

	}

}