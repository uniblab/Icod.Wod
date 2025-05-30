// Copyright (C) 2025  Timothy J. Bruce
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
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var source = this.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();
			this.Destination.WorkOrder = workOrder;
			var dest = this.Destination.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();
			this.XsltFile.WorkOrder = workOrder;
			var xslt = this.XsltFile.GetFileHandler( workOrder ) ?? throw new System.InvalidOperationException();

			var xform = new System.Xml.Xsl.XslCompiledTransform();
			using ( var xsltFileReader = xslt.OpenReader( xslt.ListFiles().First().File ) ) {
				using ( var xslReader = System.Xml.XmlReader.Create( xsltFileReader ) ) {
					xform.Load( xslReader );
				}
			}
			foreach ( var file in source.ListFiles() ) {
				DoWork( file, xform, dest );
			}
		}
		#endregion  methods


		#region static methods
		private static void DoWork( FileEntry source, System.Xml.Xsl.XslCompiledTransform xslTransform, FileHandlerBase destination ) {
			using ( var fileReader = source.Handler.OpenReader( source.File ) ) {
				using ( var xmlReader = System.Xml.XmlReader.Create( fileReader ) ) {
					DoWork( xmlReader, xslTransform, destination, source.File );
				}
			}
		}
		private static void DoWork( System.Xml.XmlReader source, System.Xml.Xsl.XslCompiledTransform xslTransform, FileHandlerBase destination, System.String filePathName ) {
			using ( var buffer = DoWork( source, xslTransform ) ) {
				destination.Overwrite( buffer, destination.FileDescriptor.GetFilePathName( destination, filePathName ) );
			}
		}
		private static System.IO.MemoryStream DoWork( System.Xml.XmlReader source, System.Xml.Xsl.XslCompiledTransform xslTransform ) {
			var output = new System.IO.MemoryStream();
			using ( var writer = System.Xml.XmlWriter.Create( output ) ) {
				xslTransform.Transform( source, writer );
				writer.Flush();
			}
			_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
			return output;
		}
		#endregion
	}

}
