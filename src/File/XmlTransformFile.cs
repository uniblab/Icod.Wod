using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"xmlTransformFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class XmlTransformFile : BinaryFileOperationBase {

		#region .ctor
		public XmlTransformFile() : base() {
		}
		public XmlTransformFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"xsltFile",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor XsltFile {
			get;
			set;
		}
		#endregion properties


		#region  methods
		public sealed override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder );
			var source = this.GetFileHandler( workOrder );
			var xslt = this.XsltFile.GetFileHandler( workOrder );
			if ( ( null == dest ) || ( null == source ) || ( null == xslt ) ) {
				throw new System.InvalidOperationException();
			}
			System.Xml.Xsl.XslCompiledTransform xform = new System.Xml.Xsl.XslCompiledTransform();
			using ( var xsltFileReader = xslt.OpenReader( xslt.ListFiles().First().File ) ) {
				using ( var xslReader = System.Xml.XmlReader.Create( xsltFileReader ) ) {
					xform.Load( xslReader );
				}
			}
			var files = source.ListFiles().Select(
				x => x.File
			);
			foreach ( var file in files ) {
				using ( var fileReader = source.OpenReader( file ) ) {
					using ( var xmlReader = System.Xml.XmlReader.Create( fileReader ) ) {
						using ( var buffer = this.DoWork( xmlReader, xform ) ) {
							var dfd = dest.FileDescriptor;
							dest.Overwrite( buffer, dfd.GetFilePathName( dest, file ) );
						}
					}
				}
			}
		}
		private System.IO.Stream DoWork( System.Xml.XmlReader source, System.Xml.Xsl.XslCompiledTransform xslTransform ) {
			var output = new System.IO.MemoryStream();
			using ( var writer = System.Xml.XmlWriter.Create( output ) ) {
				xslTransform.Transform( source, writer );
				writer.Flush();
			}
			output.Seek( 0, System.IO.SeekOrigin.Begin );
			return output;
		}
		#endregion  methods

	}

}