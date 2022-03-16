// Copyright 2022, Timothy J. Bruce
using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FileRedirection : FileBase {

		#region .ctor
		public FileRedirection() : base() {
		}
		public FileRedirection( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"append",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( false )]
		public System.Boolean Append {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void Write( WorkOrder workOrder, System.IO.Stream stream ) {
			if ( null == stream ) {
				throw new System.ArgumentNullException( "stream" );
			} else if ( !stream.CanRead ) {
				throw new System.InvalidOperationException();
			} else if ( null == workOrder ) {
				throw new System.ArgumentNullException( "workOrder" );
			}

			var handler = this.GetFileHandler( workOrder );
			handler.Overwrite( stream, handler.PathCombine( this.ExpandedPath, this.ExpandedName ) );
		}
		#endregion methods

	}

}
