// Copyright (C) 2025  Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.File {

	public abstract class FileHandlerBase {

		#region fields
		public const System.Int32 DefaultBufferLength = 16384;

		private System.Int32 myBufferLength;
		private FileDescriptor myFileDescriptor;
		[System.NonSerialized]
		private readonly Icod.Wod.WorkOrder myWorkOrder;
		#endregion fields


		#region .ctor
		protected FileHandlerBase() : base() {
			myFileDescriptor = null;
			myBufferLength = DefaultBufferLength;
		}
		protected FileHandlerBase( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : this() {
			myFileDescriptor = descriptor;
			myWorkOrder = workOrder;
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

		[System.ComponentModel.DefaultValue( DefaultBufferLength )]
		public System.Int32 BufferLength {
			get {
				return myBufferLength;
			}
			set {
				myBufferLength = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public Icod.Wod.WorkOrder WorkOrder {
			get {
				return myWorkOrder;
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
			if ( source is null ) {
				throw new System.ArgumentNullException( nameof( source ) );
			} else if ( dest is null ) {
				throw new System.ArgumentNullException( nameof( dest ) );
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
		public abstract void RmDir( System.Boolean recurse );
		public abstract void RmDir( System.String filePathName, System.Boolean recurse );

		public abstract System.Collections.Generic.IEnumerable<FileEntry> ListFiles();
		public abstract System.Collections.Generic.IEnumerable<FileEntry> ListDirectories();

		public virtual System.Collections.Generic.IEnumerable<FileEntry> List() {
			return this.ListFiles().Union( this.ListDirectories() );
		}

		public System.String PathCombine( System.String path, System.String name ) {
			return FileHelper.PathCombine( path, this.PathSeparator, name );
		}
		#endregion methods

	}

}
