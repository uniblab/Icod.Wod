// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FileRedirection : FileBase {

		#region .ctor
		public FileRedirection() : base() {
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
			stream = stream ?? throw new System.ArgumentNullException( nameof( stream ) );
			workOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			if ( !stream.CanRead ) {
				throw new System.InvalidOperationException();
			}

			var handler = this.GetFileHandler( workOrder );
			handler.Overwrite( stream, handler.PathCombine( this.ExpandedPath, this.ExpandedName ) );
		}
		#endregion methods

	}

}
