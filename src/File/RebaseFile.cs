using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"rebaseFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class RebaseFile : FileOperationBase {

		#region fields
		private System.String myOutputCodePage;
		#endregion fields


		#region .ctor
		public RebaseFile() : base() {
			myOutputCodePage = DefaultCodePage;
		}
		public RebaseFile( WorkOrder workOrder ) : base( workOrder ) {
			myOutputCodePage = DefaultCodePage;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"outputCodePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultCodePage )]
		public System.String OutputCodePage {
			get {
				return myOutputCodePage;
			}
			set {
				myOutputCodePage = value;
			}
		}
		#endregion properties


		#region methods
		public sealed override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var sourceHandler = this.GetFileHandler( workOrder );
			if ( null == sourceHandler ) {
				throw new System.InvalidOperationException();
			}

			var sourceEncoding = CodePageHelper.GetCodePage( this.CodePage );
			var destEncoding = CodePageHelper.GetCodePage( this.OutputCodePage );
			foreach ( var file in sourceHandler.ListFiles().Select(
				x => x.File
			) ) {
				using ( var buffer = new System.IO.MemoryStream( this.BufferLength ) ) {
					using ( var source = sourceHandler.OpenReader( file ) ) {
						using ( var reader = new System.IO.StreamReader( source, sourceEncoding, true, this.BufferLength, true ) ) {
							using ( var writer = new System.IO.StreamWriter( buffer, destEncoding, this.BufferLength, true ) ) {
								var rs = this.RecordSeparator;
								System.String line = reader.ReadLine( rs );
								while ( null != line ) {
									writer.Write( line + rs );
									line = reader.ReadLine( rs );
								}
							}
						}
					}
					buffer.Seek( 0, System.IO.SeekOrigin.Begin );
					sourceHandler.Overwrite( buffer, file );
				}
			}
		}
		#endregion methods

	}

}