using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlRoot(
		ElementName = "WorkOrder",
		Namespace = "http://Icod.Wod",
		IsNullable = false
	)]
	public class WorkOrder {

		#region fields
		private ConnectionStringEntry[] myConnectionStrings;
		private System.Object[] mySteps;
		[System.NonSerialized]
		private readonly System.Collections.Generic.IDictionary<System.String,System.String> myDict;
		#endregion fields


		#region .ctor
		public WorkOrder() : base() {
			myDict = new System.Collections.Generic.Dictionary<System.String,System.String>( System.StringComparer.OrdinalIgnoreCase );

			myDict.Add( "%wod:File-PathName%", System.String.Empty );
			myDict.Add( "%wod:File-Path%", System.String.Empty );
			myDict.Add( "%wod:File-Name%", System.String.Empty );
			myDict.Add( "%wod:File-NameWithoutExtension%", System.String.Empty );
		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"jobName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String JobName {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"emailTo",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.String EmailTo {
			get;
			set;
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "connectionStrings"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"connectionString",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public ConnectionStringEntry[] ConnectionStrings {
			get {
				return myConnectionStrings;
			}
			set {
				myConnectionStrings = value;
			}
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "steps"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Email ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( File.FileOperationBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.DbOperationBase ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileExport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileImport ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.DbCommand ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		public System.Object[] Steps {
			get {
				return mySteps;
			}
			set {
				mySteps = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Collections.Generic.IDictionary<System.String,System.String> VarDictionary {
			get {
				return myDict;
			}
		}
		#endregion properties


		#region methods
		public void Run() {
			System.Int32 i = 1;
			IStep step = null;
			try {
				var steps = ( this.Steps ?? new System.Object[ 0 ] ).OfType<IStep>().ToArray();
				foreach ( var s in steps ) {
					step = s;
					s.DoWork( this );
					i++;
				}
			} catch ( System.Exception e ) {
				throw new Icod.Wod.Exception( e.Message, e, i, this, step );
			}
		}

		public System.String ExpandVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			if ( @string.Contains( "%wod:DateTime-" ) ) {
				var now = System.DateTime.Now;
				var yy = now.ToString( "yy" );
				while ( @string.Contains( "%wod:DateTime-yy%" ) ) {
					@string = @string.Replace( "%wod:DateTime-yy%", yy );
				}
				var yyyy = now.ToString( "yyyy" );
				while ( @string.Contains( "%wod:DateTime-yyyy%" ) ) {
					@string = @string.Replace( "%wod:DateTime-yyyy%", yyyy );
				}
				var MM = now.ToString( "MM" );
				while ( @string.Contains( "%wod:DateTime-MM%" ) ) {
					@string = @string.Replace( "%wod:DateTime-MM%", MM );
				}
				var dd = now.ToString( "dd" );
				while ( @string.Contains( "%wod:DateTime-dd%" ) ) {
					@string = @string.Replace( "%wod:DateTime-dd%", dd );
				}
				var yyyyMMdd = now.ToString( "yyyyMMdd" );
				while ( @string.Contains( "%wod:DateTime-yyyyMMdd%" ) ) {
					@string = @string.Replace( "%wod:DateTime-yyyyMMdd%", yyyyMMdd );
				}
			}
			@string = @string.Replace( "%wod:EmailTo%", this.EmailTo );
			@string = @string.Replace( "%wod:JobName%", this.JobName );
			return @string;
		}
		#endregion methods

	}

}