using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( RmZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( MkZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryZipOperationBase ) )]
	[System.Xml.Serialization.XmlType(
		"zipOperation",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public abstract class ZipOperationBase : FileOperationBase {

		#region fields
		private System.String myCodePage;
		private System.Boolean myTruncateEntryName;
		private System.Boolean myWriteIfEmpty;
		#endregion fields


		#region .ctor
		protected ZipOperationBase() : base() {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
			myWriteIfEmpty = true;
		}
		protected ZipOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
			myWriteIfEmpty = true;
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"codePage",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( "windows-1252" )]
		public virtual System.String CodePage {
			get {
				return myCodePage;
			}
			set {
				myCodePage = value;
			}
		}

		[System.Xml.Serialization.XmlAttribute(
			"truncateEntryName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public virtual System.Boolean TruncateEntryName {
			get {
				return myTruncateEntryName;
			}
			set {
				myTruncateEntryName = value;
			}
		}
		[System.Xml.Serialization.XmlAttribute(
			"writeIfEmpty",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( true )]
		public virtual System.Boolean WriteIfEmpty {
			get {
				return myWriteIfEmpty;
			}
			set {
				myWriteIfEmpty = value;
			}
		}

		[System.Xml.Serialization.XmlElement(
			"source",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		public virtual FileDescriptor Source {
			get;
			set;
		}
		#endregion properties


		#region methods
		public virtual System.Text.Encoding GetEncoding() {
			var cp = this.CodePage;
			System.Int32 cpNumber;
			if ( System.Int32.TryParse( cp, out cpNumber ) ) {
				return System.Text.Encoding.GetEncoding( cpNumber );
			} else {
				return System.Text.Encoding.GetEncoding( cp );
			}
		}

		protected virtual System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> MatchEntries( System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> collection ) {
			if ( null == collection ) {
				throw new System.ArgumentNullException( "collection" );
			}

			var source = this.Source;
			if ( null == source ) {
				throw new System.InvalidOperationException();
			}
			var regexPattern = source.RegexPattern;
			var regexMatch = System.String.IsNullOrEmpty( regexPattern )
				? collection
				: collection.Where(
					x => System.Text.RegularExpressions.Regex.IsMatch( x.FullName, regexPattern )
				)
			;
			var path = source.ExpandedPath;
			var dirMatch = System.String.IsNullOrEmpty( path )
				? regexMatch
				: regexMatch.Where(
					x => x.FullName.StartsWith( path )
				)
			;
			var name = source.ExpandedName;
			var nameMatch = System.String.IsNullOrEmpty( name )
				? dirMatch
				: dirMatch.Where(
					x => x.Name.Equals( name )
				)
			;
			return nameMatch;
		}

		protected virtual System.IO.Compression.ZipArchive GetZipArchive( System.IO.Stream stream, System.IO.Compression.ZipArchiveMode zipArchiveMode ) {
			if ( null == stream ) {
				throw new System.ArgumentNullException( "stream" );
			}
			return new System.IO.Compression.ZipArchive( stream, zipArchiveMode, true, this.GetEncoding() );
		}
		#endregion methods

	}

}