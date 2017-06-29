using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlRoot(
		ElementName = "WorkOrder",
		Namespace = "http://Icod.Wod",
		IsNullable = false
	)]
	public sealed class WorkOrder {

		#region fields
		private const System.String DateTimeFormat = @"(?<DateTimeFormat>%wod:DateTime\{(?<dateTimeFormatString>[^\}]+)\}%)";

		private ConnectionStringEntry[] myConnectionStrings;
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
			typeof( Parallel ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Serial ),
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( File.FileOperationBase ),
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
			get;
			set;
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
				var w = new System.ApplicationException( e.Message, e );
				w.Data.Add( "StepNumber", i );
				w.Data.Add( "WorkOrder", this );
				w.Data.Add( "Step", step );
				throw w;
			}
		}

		public System.String ExpandVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = System.Environment.ExpandEnvironmentVariables( @string );
			foreach ( System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches( @string, DateTimeFormat ) ) {
				@string = System.Text.RegularExpressions.Regex.Replace( @string, m.Value, System.DateTime.Now.ToString( m.Groups[ "dateTimeFormatString" ].Value ) );
			}
			@string = @string.Replace( "%wod:EmailTo%", this.EmailTo );
			@string = @string.Replace( "%wod:JobName%", this.JobName );
			@string = System.Environment.ExpandEnvironmentVariables( @string );
			return @string;
		}
		#endregion methods

	}

}