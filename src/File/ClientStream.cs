using System.Linq;

namespace Icod.Wod.File {

	public sealed class ClientStream : System.IO.Stream {

		#region fields
		private System.IO.Stream myStream;
		private System.IDisposable myClient;
		private readonly System.Boolean myDisposeClient;
		#endregion fields


		#region .ctor
		private ClientStream() : base() {
		}
		public ClientStream( System.IO.Stream stream, System.IDisposable client, System.Boolean disposeClient ) : this() {
			if ( null == client ) {
				throw new System.ArgumentNullException( "client" );
			} else if ( null == stream ) {
				throw new System.ArgumentNullException( "stream" );
			}
			myClient = client;
			myStream = stream;
			myDisposeClient = disposeClient;
		}
		public ClientStream( System.IO.Stream stream, System.IDisposable client ) : this( stream, client, true ) {
		}

		~ClientStream() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region properties
		public override System.Boolean CanRead {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanRead;
			}
		}
		public override System.Boolean CanSeek {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanSeek;
			}
		}
		public override System.Boolean CanWrite {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanWrite;
			}
		}

		public override System.Int64 Length {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.Length;
			}
		}
		public override System.Int64 Position {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.Position;
			}
			set {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				myStream.Position = value;
			}
		}
		#endregion properties


		#region methods
		protected override void Dispose( System.Boolean disposing ) {
			if ( disposing ) {
				if ( null != myStream ) {
					myStream.Dispose();
					myStream = null;
				}
				if ( myDisposeClient && ( null != myClient ) ) {
					myClient.Dispose();
					myClient = null;
				}
			}
			base.Dispose( disposing );
		}

		public override void Flush() {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Flush();
		}
		public override System.Int64 Seek( System.Int64 offset, System.IO.SeekOrigin origin ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Seek( offset, origin );
		}
		public override void SetLength( System.Int64 value ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.SetLength( value );
		}
		public override System.Int32 Read( [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Read( buffer, offset, count );
		}
		public override void Write( System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Read( buffer, offset, count );
		}
		#endregion methods

	}

}