using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	public sealed class FileRedirection : FileDescriptor {

		#region fields
		private System.String myCodePage;
		#endregion fields


		#region .ctor
		public FileRedirection() : base() {
			myCodePage = "windows-1252";
		}
		public FileRedirection( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "windows-1252" )]
		public System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}
		#endregion properties


		#region methods
		public System.Text.Encoding GetEncoding() {
			return CodePageHelper.GetCodePage( this.CodePage );
		}
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