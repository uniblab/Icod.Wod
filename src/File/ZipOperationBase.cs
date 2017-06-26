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
		#endregion fields


		#region .ctor
		protected ZipOperationBase() : base() {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
		}
		protected ZipOperationBase( WorkOrder workOrder ) : base( workOrder ) {
			myCodePage = "windows-1252";
			myTruncateEntryName = true;
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

		protected virtual System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> ListEntries( System.IO.Compression.ZipArchive zip, FileDescriptor source ) {
			if ( null == zip ) {
				throw new System.ArgumentNullException( "zip" );
			} else if ( null == source ) {
				throw new System.ArgumentNullException( "source" );
			}

			var regexPattern = source.RegexPattern;
			var entries = zip.Entries;
			var regexMatch = System.String.IsNullOrEmpty( regexPattern )
				? entries
				: entries.Where(
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
			return System.String.IsNullOrEmpty( name )
				? dirMatch
				: dirMatch.Where(
					x => x.Name.Equals( name )
				)
			;
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