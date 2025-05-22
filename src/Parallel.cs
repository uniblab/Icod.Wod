// Copyright (C) 2025  Timothy J. Bruce
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
			typeof( Data.DbCommand ),
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

		[System.Xml.Serialization.XmlIgnore]
		public WorkOrder WorkOrder {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( nameof( workOrder ) );
			var steps = ( this.Steps ?? System.Array.Empty<IStep>() ).OfType<IStep>();
			if ( !steps.Any() ) {
				return;
			}
			using ( var tokenSource = new System.Threading.CancellationTokenSource() ) {
				var token = tokenSource.Token;
				var maxP = this.MaxDegreeOfParallelism;
				if ( DefaultMaxDegreeOfParallelism == maxP ) {
					DoUnlimitedWork( workOrder, steps, token );
				} else {
					this.DoLimitedWork( workOrder, steps, token );
				}
			}
		}
		private void DoLimitedWork( WorkOrder workOrder, System.Collections.Generic.IEnumerable<IStep> steps, System.Threading.CancellationToken token ) {
			using ( var semaphore = new Semaphore( this.MaxDegreeOfParallelism, this.MaxDegreeOfParallelism ) ) {
				System.Collections.Generic.List<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
				var factory = new System.Threading.Tasks.TaskFactory( token );
				foreach ( var step in steps ) {
					tasks.Add( factory.StartNew(
						() => {
							semaphore.Wait();
							step.DoWork( workOrder );
							_ = semaphore.Release();
						},
						token
					) );
				}
				System.Threading.Tasks.Task.WaitAll( tasks.ToArray(), token );
			}
		}
		#endregion methods


		#region static methods
		private static void DoUnlimitedWork( WorkOrder workOrder, System.Collections.Generic.IEnumerable<IStep> steps, System.Threading.CancellationToken token ) {
			System.Collections.Generic.List<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
			var factory = new System.Threading.Tasks.TaskFactory( token );
			foreach ( var step in steps ) {
				tasks.Add( factory.StartNew(
					() => step.DoWork( workOrder ),
					token
				) );
			}
			System.Threading.Tasks.Task.WaitAll( tasks.ToArray(), token );
		}
		#endregion

	}

}
