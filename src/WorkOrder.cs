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
				return System.String.Join( ",", ( this.Email ?? new System.String[ 0 ] ).Union( new System.String[ 1 ] { this.EmailTo ?? System.String.Empty } ).Where(
					x => !System.String.IsNullOrEmpty( x )
				) ) ?? System.String.Empty;
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
			typeof( Data.DbOperationBase ),
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

		private System.String ExpandEnvironmentVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			return System.Environment.ExpandEnvironmentVariables( @string );
		}
		private System.String ExpandWodVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			foreach ( System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches( @string, WorkOrder.DateTimeFormat ) ) {
				@string = System.Text.RegularExpressions.Regex.Replace( @string, m.Value, System.DateTime.Now.ToString( m.Groups[ "dateTimeFormatString" ].Value ) );
			}
			@string = @string.Replace( "%wod:EmailTo%", this.EmailList );
			@string = @string.Replace( "%wod:JobName%", this.JobName );
			return @string;
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
		public System.String ExpandPseudoVariables( System.String @string ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			@string = this.ExpandWorkOrderVariables( @string );
			@string = this.ExpandWodVariables( @string );
			@string = this.ExpandEnvironmentVariables( @string );
			return @string;
		}
		public System.String ExpandPseudoVariables( System.String @string, IStack<ContextRecord> context ) {
			@string = @string.TrimToNull();
			if ( System.String.IsNullOrEmpty( @string ) ) {
				return null;
			}
			System.Collections.Generic.IEnumerable<System.Data.DataColumn> cols;
			foreach ( var rec in ( context ?? Stack<ContextRecord>.Empty ).Where(
				x => ( null != x.Record )
			).Select(
				x => x.Record
			).Where(
				x => ( null != x ) && ( null != x.Table ) && ( null != x.Table.Columns )
			) ) {
				cols = rec.Table.Columns.OfType<System.Data.DataColumn>();
				if ( ( null == cols ) || !cols.Any() ) {
					continue;
				}
				foreach ( var col in cols ) {
					@string = @string.Replace( "%rec:" + col.ColumnName + "%", ( rec[ col ] ?? System.String.Empty ).ToString() );
				}
			}
			@string = this.ExpandPseudoVariables( @string );
			return @string;
		}

		public sealed override System.String ToString() {
			return this.JobName ?? base.ToString();
		}
		#endregion methods

	}

}