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
		private const System.String AppSetting = @"(?<AppSetting>%app:(?<AppKeyName>[^%]+)%)";
		private const System.String CmdArgFormat = @"(?<CmdArgFormat>%cmd:(?<CmdArgNumber>\d+)%)";

		private static System.String[] theCmdArgs;
		#endregion fields


		#region .ctor
		public WorkOrder() : base() {
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
		[System.Xml.Serialization.XmlIgnore]
		private System.String ExpandedEmailTo {
			get {
				return this.ExpandPseudoVariables( this.EmailTo );
			}
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
			get;
			set;
		}

		[System.Xml.Serialization.XmlArray(
			ElementName = "sfCredentials",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"sfCredential",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public SalesForce.Credential[] SFCredentials {
			get;
			set;
		}

		[System.Xml.Serialization.XmlArray(
			ElementName = "variables",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"variable",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public Variable[] Variables {
			get;
			set;
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "emails"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			"email",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlIgnore]
		[System.ComponentModel.DefaultValue( null )]
		public System.String[] Email {
			get;
			set;
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.String EmailList {
			get {
				return System.String.Join(
					",",
					(
						( this.Email ?? new System.String[ 0 ] ).Select(
							x => this.ExpandPseudoVariables( x )
						) ?? new System.String[ 0 ]
					).Union(
						new System.String[ 1 ] { this.ExpandedEmailTo ?? System.String.Empty }
					).Where(
						x => !System.String.IsNullOrEmpty( x )
					).Distinct(
						System.StringComparer.OrdinalIgnoreCase
					)
				) ?? System.String.Empty;
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
			ElementName = "dbFileExport",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.FileImport ),
			ElementName = "dbFileImport",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( Data.Command ),
			ElementName = "dbCommand",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Rest.RestSelect ),
			ElementName = "sfRestSelect",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.BulkAggregateOperation ),
			ElementName = "sfBulkOperation",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.Object[] Steps {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void Run() {
			theCmdArgs = System.Environment.GetCommandLineArgs() ?? new System.String[ 0 ];
			System.Int32 i = 1;
			IStep step = null;
			try {
				var steps = ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>().ToArray();
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

		private System.String ExpandWorkOrderVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			foreach ( var @var in ( this.Variables ?? new Variable[ 0 ] ) ) {
				@string = @string.Replace( "%var:" + @var.Name + "%", @var.Value ?? System.String.Empty );
			}
			return @string;
		}
		private System.String ExpandWodVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			foreach ( System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches( @string, WorkOrder.DateTimeFormat ) ) {
				@string = System.Text.RegularExpressions.Regex.Replace( @string, m.Value, System.DateTime.Now.ToString( m.Groups[ "dateTimeFormatString" ].Value ) );
			}
			@string = @string.Replace( "%wod:EmailTo%", this.EmailTo );
			@string = @string.Replace( "%wod:JobName%", this.JobName );
			return @string;
		}
		private System.String ExpandAppVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			var settings = System.Configuration.ConfigurationManager.AppSettings;
			foreach ( System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches( @string, WorkOrder.AppSetting ) ) {
				@string = System.Text.RegularExpressions.Regex.Replace( @string, m.Value, settings[ m.Groups[ "AppKeyName" ].Value ] ?? System.String.Empty );
			}
			return @string;
		}
		private System.String ExpandEnvironmentVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return System.Environment.ExpandEnvironmentVariables( @string );
		}
		private System.String ExpandCmdVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}

			foreach ( System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches( @string, WorkOrder.CmdArgFormat ) ) {
				@string = System.Text.RegularExpressions.Regex.Replace( @string, m.Value, theCmdArgs[ System.Int32.Parse( m.Groups[ "CmdArgNumber" ].Value ) ] );
			}
			return @string;
		}
		public System.String ExpandPseudoVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = this.ExpandWorkOrderVariables( @string );
			@string = this.ExpandWodVariables( @string );
			@string = this.ExpandAppVariables( @string );
			@string = this.ExpandEnvironmentVariables( @string );
			return @string;
		}

		public sealed override System.String ToString() {
			return this.JobName ?? base.ToString();
		}
		#endregion methods

	}

}