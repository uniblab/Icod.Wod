// Copyright (C) 2025  Timothy J. Bruce

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
			client  = client ?? throw new System.ArgumentNullException( nameof( client ) );
			stream = stream ?? throw new System.ArgumentNullException( nameof( stream ) );
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
				if ( myStream is null ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanRead;
			}
		}
		public sealed override System.Boolean CanSeek {
			get {
				if ( myStream is null ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanSeek;
			}
		}
		public sealed override System.Boolean CanWrite {
			get {
				if ( myStream is null ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.CanWrite;
			}
		}

		public sealed override System.Int64 Length {
			get {
				if ( myStream is null ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.Length;
			}
		}
		public sealed override System.Int64 Position {
			get {
				if ( myStream is null ) {
					throw new System.ObjectDisposedException( null );
				}
				return myStream.Position;
			}
			set {
				if ( myStream is null ) {
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
			if ( myStream is null ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Flush();
		}
		public sealed override System.Int64 Seek( System.Int64 offset, System.IO.SeekOrigin origin ) {
			if ( myStream is null ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Seek( offset, origin );
		}
		public sealed override void SetLength( System.Int64 value ) {
			if ( myStream is null ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.SetLength( value );
		}
		public sealed override System.Int32 Read( [System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( myStream is null ) {
				throw new System.ObjectDisposedException( null );
			}
			return myStream.Read( buffer, offset, count );
		}
		public sealed override void Write( System.Byte[] buffer, System.Int32 offset, System.Int32 count ) {
			if ( myStream is null ) {
				throw new System.ObjectDisposedException( null );
			}
			myStream.Write( buffer, offset, count );
		}
		#endregion methods

	}

}
