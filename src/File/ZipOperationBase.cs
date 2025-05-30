// Copyright (C) 2025  Timothy J. Bruce
using System;
using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( AddZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( MkZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( RmZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( TouchZip ) )]
	[System.Xml.Serialization.XmlInclude( typeof( BinaryZipOperationBase ) )]
	[System.Xml.Serialization.XmlType(
		"zipOperation",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public abstract class ZipOperationBase : FileOperationBase, ITruncateEntryName {

		#region fields
		private static readonly System.Func<FileEntry, System.String, System.String> theTruncatedGetFileName;
		private static readonly System.Func<FileEntry, System.String, System.String> theFullGetFileName;

		private System.Boolean myTruncateEntryName;
		private System.Func<FileEntry, System.String, System.String> myGetFileName;
		#endregion fields


		#region .ctor
		static ZipOperationBase() {
			theTruncatedGetFileName = ( x, y ) => System.IO.Path.GetFileName( x.File );
			theFullGetFileName = ( x, y ) => x.File.Replace( y, System.String.Empty );
		}

		protected ZipOperationBase() : base() {
			myTruncateEntryName = true;
			myGetFileName = theTruncatedGetFileName;
		}
		#endregion .ctor


		#region properties
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
				myGetFileName = ( value )
					? theTruncatedGetFileName
					: theFullGetFileName
				;
			}
		}

		[System.Xml.Serialization.XmlArray(
			ElementName = "sources",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		[System.Xml.Serialization.XmlArrayItem(
			Type = typeof( FileDescriptor ),
			ElementName = "source",
			Namespace = "http://Icod.Wod",
			IsNullable = false
		)]
		[System.ComponentModel.DefaultValue( null )]
		public virtual FileDescriptor[] Source {
			get;
			set;
		}
		#endregion properties


		#region methods
		protected virtual System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> MatchEntries( System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> collection ) {
			return ( this.Source ?? System.Array.Empty<FileDescriptor>() ).Select(
				x => {
					x.WorkOrder = this.WorkOrder;
					return x;
				}
			).SelectMany(
				x => this.MatchEntries( collection, x )
			);
		}
		protected virtual System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> MatchEntries( System.Collections.Generic.IEnumerable<System.IO.Compression.ZipArchiveEntry> collection, FileDescriptor source ) {
			source = source ?? throw new System.ArgumentNullException( nameof( source ) );
			collection = collection ?? throw new System.ArgumentNullException( nameof( collection ) );

			if ( source is null ) {
				throw new System.InvalidOperationException();
			}
			var regexPattern = source.ExpandedRegexPattern;
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
			stream = stream ?? throw new System.ArgumentNullException( nameof( stream ) );
			return new System.IO.Compression.ZipArchive( stream, zipArchiveMode, true, this.GetEncoding() );
		}

		protected virtual System.String ProcessFileName( FileEntry file, System.String sourceExpandedPath ) {
			var output = myGetFileName( file, sourceExpandedPath ).Replace( '\\', '/' );
			while ( !System.String.IsNullOrEmpty( output ) && output.StartsWith( "/", StringComparison.OrdinalIgnoreCase ) ) {
				output = output.Substring( 1 );
			}
			return output;
		}
		#endregion methods

	}

}
