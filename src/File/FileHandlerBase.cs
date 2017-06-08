using System.Linq;

namespace Icod.Wod.File {

	public abstract class FileHandlerBase {

		#region fields
		private System.Int32 myBufferLength;
		private FileDescriptor myFileDescriptor;
		#endregion fields


		#region .ctor
		protected FileHandlerBase() : base() {
			myFileDescriptor = null;
			myBufferLength = 16384;
		}
		protected FileHandlerBase( FileDescriptor descriptor ) : this() {
			myFileDescriptor = descriptor;
		}
		#endregion .ctor


		#region properties
		public abstract System.Char PathSeparator {
			get;
		}
		public FileDescriptor FileDescriptor {
			get {
				return myFileDescriptor;
			}
			set {
				myFileDescriptor = value;
			}
		}

		[System.ComponentModel.DefaultValue( 16384 )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}
		#endregion properties


		#region methods
		public abstract void TouchFile();
		public abstract void DeleteFile();
		public abstract void DeleteFile( System.String filePathName );

		public abstract System.IO.Stream OpenReader( System.String filePathName );
		public abstract void Overwrite( System.IO.Stream source, System.String filePathName );
		public abstract void Append( System.IO.Stream source, System.String filePathName );

		protected System.Int64 Write( System.IO.Stream source, System.IO.Stream dest ) {
			if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			} else if ( null == dest ) {
				throw new System.ArgumentNullException( "dest" );
			}
			System.Int64 total = 0;

			var buffer = new System.Byte[ this.BufferLength ];
			System.Int32 r;
			do {
				r = source.Read( buffer, 0, this.BufferLength );
				if ( 0 < r ) {
					dest.Write( buffer, 0, r );
				}
				total += r;
			} while ( 0 < r );

			return total;
		}

		public abstract void MkDir();
		public abstract void RmDir();

		public abstract System.Collections.Generic.IEnumerable<FileEntry> ListFiles();
		public abstract System.Collections.Generic.IEnumerable<FileEntry> ListDirectories();

		public virtual System.Collections.Generic.IEnumerable<FileEntry> List() {
			return this.ListFiles().Union( this.ListDirectories() );
		}

		public System.String PathCombine( System.String path, System.String name ) {
			if ( System.String.IsNullOrEmpty( name ) ) {
				throw new System.ArgumentNullException( "name" );
			} else if ( System.String.IsNullOrEmpty( path ) ) {
				throw new System.ArgumentNullException( "path" );
			}
			var sep = this.PathSeparator.ToString();
			while ( path.EndsWith( sep ) ) {
				path = path.Substring( 0, path.Length - 1 );
			}
			while ( name.StartsWith( sep ) ) {
				sep = sep.Substring( 1 );
			}
			return path + sep + name;
		}
		#endregion methods

	}

}