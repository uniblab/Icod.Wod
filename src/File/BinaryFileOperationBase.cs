// Copyright (C) 2025  Timothy J. Bruce
namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlInclude( typeof( AppendFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CopyFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( JsonToXml ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PreambleFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PrefixFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( PruneFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RebaseFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RenameFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( SuffixFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( XmlToJson ) )]
	[System.Xml.Serialization.XmlInclude( typeof( XmlTransformFile ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryCompressedFileOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( CountBinaryFileOperationBase ) )]
	[System.Xml.Serialization.XmlInclude( typeof( FileOrDirectoryListerBase ) )]
	[System.Xml.Serialization.XmlType(
		"binaryFileOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class BinaryFileOperationBase : FileOperationBase, IDestination {

		#region .ctor
		protected BinaryFileOperationBase() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public FileDescriptor Destination {
			get;
			set;
		}
		#endregion properties

	}

}
