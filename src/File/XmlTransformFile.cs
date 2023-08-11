// Icod.Wod is the Work on Demand framework.
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
		public sealed override void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
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
				this.DoWork( file, xform, dest );
			}
		}
		private void DoWork( FileEntry source, System.Xml.Xsl.XslCompiledTransform xslTransform, FileHandlerBase destination ) {
			using ( var fileReader = source.Handler.OpenReader( source.File ) ) {
				using ( var xmlReader = System.Xml.XmlReader.Create( fileReader ) ) {
					this.DoWork( xmlReader, xslTransform, destination, source.File );
				}
			}
		}
		private void DoWork( System.Xml.XmlReader source, System.Xml.Xsl.XslCompiledTransform xslTransform, FileHandlerBase destination, System.String filePathName ) {
			using ( var buffer = this.DoWork( source, xslTransform ) ) {
				destination.Overwrite( buffer, destination.FileDescriptor.GetFilePathName( destination, filePathName ) );
			}
		}
		private System.IO.Stream DoWork( System.Xml.XmlReader source, System.Xml.Xsl.XslCompiledTransform xslTransform ) {
			var output = new System.IO.MemoryStream();
			using ( var writer = System.Xml.XmlWriter.Create( output ) ) {
				xslTransform.Transform( source, writer );
				writer.Flush();
			}
			_ = output.Seek( 0, System.IO.SeekOrigin.Begin );
			return output;
		}
		#endregion  methods

	}

}
