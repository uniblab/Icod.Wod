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

using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"appendFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class AppendFile : BinaryFileOperationBase, IMove {

		#region fields
		private System.Boolean myMove;
		#endregion fields


		#region .ctor
		public AppendFile() : base() {
			myMove = false;
		}
		public AppendFile( Icod.Wod.WorkOrder workOrder ) : base( workOrder ) {
			myMove = false;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"move",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Move {
			get {
				return myMove;
			}
			set {
				myMove = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var dfd = dest.FileDescriptor;
			var source = this.GetFileHandler( workOrder );
			System.Action<FileHandlerBase, System.String> delFile;
			if ( this.Move ) {
				delFile = ( s, f ) => s.DeleteFile( f );
			} else {
				delFile = ( s, f ) => {
				};
			}
			var files = source.ListFiles().Select(
				x => x.File
			);
			foreach ( var file in files ) {
				using ( var reader = source.OpenReader( file ) ) {
					dest.Append( reader, dfd.GetFilePathName( dest, file ) );
				}
				delFile( source, file );
			}
		}
		#endregion methods

	}

}
