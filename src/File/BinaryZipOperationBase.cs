// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( FromZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( ListZip ) )]
	[System.Xml.Serialization.XmlType(
		"binaryZipOperation",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public abstract class BinaryZipOperationBase : ZipOperationBase, IDestination {

		#region .ctor
		protected BinaryZipOperationBase() : base() {
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlElement(
			"destination",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public virtual FileDescriptor Destination {
			get;
			set;
		}
		#endregion properties

	}

}
