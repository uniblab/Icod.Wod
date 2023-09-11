// Icod.Wod.dll is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

using System.IO;
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
		protected FileHandlerBase( Icod.Wod.WorkOrder workOrder ) : this() {
			myWorkOrder = workOrder;
		}
		protected FileHandlerBase( Icod.Wod.WorkOrder workOrder, FileDescriptor descriptor ) : this( workOrder ) {
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
