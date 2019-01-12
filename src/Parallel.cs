using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"parallel",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Parallel : IStep {

		#region fields
		public const System.Int32 DefaultMaxConcurrency = -1;
		#endregion fields


		#region .ctor
		public Parallel() : base() {
			this.MaxConcurrency = DefaultMaxConcurrency;
		}
		#endregion .ctor


		#region properties
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
		public System.Object[] Steps {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"maxConcurrency",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultMaxConcurrency )]
		public System.Int32 MaxConcurrency {
			get;
			set;
		}

		[System.Xml.Serialization.XmlIgnore]
		public IStack<ContextRecord> Context {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			this.DoWork( workOrder,  Stack<ContextRecord>.Empty );
		}
		public void DoWork( Icod.Wod.WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			var steps = ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>().Select<IStep, System.Action>(
				x => () => x.DoWork( workOrder )
			);
			if ( steps.Any() ) {
				System.Threading.Tasks.Parallel.Invoke(
					new System.Threading.Tasks.ParallelOptions {
						MaxDegreeOfParallelism = this.MaxConcurrency
					},
					steps.ToArray()
				);
			}
		}
		#endregion methods

	}

}