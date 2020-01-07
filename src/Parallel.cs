using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"parallel",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Parallel : IStep {

		#region fields
		public const System.Int32 DefaultMaxDegreeOfParallelism = -1;
		#endregion fields


		#region .ctor
		public Parallel() : base() {
			this.MaxDegreeOfParallelism = DefaultMaxDegreeOfParallelism;
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
			typeof( SalesForce.Bulk.BulkSelect ),
			ElementName = "sfBulkSelect",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public System.Object[] Steps {
			get;
			set;
		}

		[System.Xml.Serialization.XmlAttribute(
			"maxDegreeOfParallelism",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( DefaultMaxDegreeOfParallelism )]
		public System.Int32 MaxDegreeOfParallelism {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			var steps = ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>();
			if ( !steps.Any() ) {
				return;
			}
			var tokenSource = new System.Threading.CancellationTokenSource();
			var token = tokenSource.Token;
			var tasks = steps.Select(
				x => new System.Threading.Tasks.Task(
					() => {
						try {
							x.DoWork( workOrder );
						} catch {
							tokenSource.Cancel( true );
							throw;
						}
					},
					token
				)
			);

			System.Threading.Tasks.Parallel.Invoke(
				new System.Threading.Tasks.ParallelOptions {
					MaxDegreeOfParallelism = this.MaxDegreeOfParallelism,
					CancellationToken = token
				},
				tasks.Select<System.Threading.Tasks.Task, System.Action>(
					x => () => {
						try {
							x.RunSynchronously();
						} catch {
							tokenSource.Cancel( true );
							throw;
						}
					}
				).ToArray()
			);
		}
		#endregion methods

	}

}