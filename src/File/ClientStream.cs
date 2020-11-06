// Copyright 2020, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public sealed class ClientStream : System.IO.Stream {

		#region fields
		private readonly System.IO.Stream myStream;
		private readonly System.IDisposable myClient;
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
		public sealed override System.Boolean CanRead {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanRead;
			}
		}
		public sealed override System.Boolean CanSeek {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanSeek;
			}
		}
		public sealed override System.Boolean CanWrite {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanWrite;
			}
		}

		public sealed override System.Int64 Length {
			get {
				if ( null == myStream ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.Length;
			}
		}
		public sealed override System.Int64 Position {
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
		protected sealed override void Dispose( System.Boolean disposing ) {
			if ( disposing ) {
				myStream.Dispose();
				if ( myDisposeClient ) {
					myClient.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public sealed override void Flush() {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Flush();
		}
		public sealed override System.Int64 Seek( System.Int64 offset, System.IO.SeekOrigin origin ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Seek( offset, origin );
		}
		public sealed override void SetLength( System.Int64 value ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.SetLength( value );
		}
		public sealed override System.Int32 Read( [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Read( buffer, offset, count );
		}
		public sealed override void Write( System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( null == myStream ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Write( buffer, offset, count );
		}
		#endregion methods

	}

}
