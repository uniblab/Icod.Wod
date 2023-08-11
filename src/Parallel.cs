// Icod.Wod is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

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
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var steps = ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>();
			if ( !steps.Any() ) {
				return;
			}
			using ( var tokenSource = new System.Threading.CancellationTokenSource() ) {
				var token = tokenSource.Token;
				var maxP = this.MaxDegreeOfParallelism;
				if ( DefaultMaxDegreeOfParallelism == maxP ) {
					this.DoUnlimitedWork( workOrder, steps, token );
				} else {
					this.DoLimitedWork( workOrder, steps, token );
				}
			}
		}
		private void DoUnlimitedWork( WorkOrder workOrder, System.Collections.Generic.IEnumerable<IStep> steps, System.Threading.CancellationToken token ) {
			System.Collections.Generic.ICollection<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
			var factory = new System.Threading.Tasks.TaskFactory( token );
			foreach ( var step in steps ) {
				tasks.Add( factory.StartNew(
					() => step.DoWork( workOrder ),
					token
				) );
			}
			System.Threading.Tasks.Task.WaitAll( tasks.ToArray(), token );
		}
		private void DoLimitedWork( WorkOrder workOrder, System.Collections.Generic.IEnumerable<IStep> steps, System.Threading.CancellationToken token ) {
			using ( var semaphore = new Semaphore( this.MaxDegreeOfParallelism, this.MaxDegreeOfParallelism ) ) {
				System.Collections.Generic.ICollection<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
				var factory = new System.Threading.Tasks.TaskFactory( token );
				foreach ( var step in steps ) {
					tasks.Add( factory.StartNew(
						() => {
							semaphore.Wait();
							step.DoWork( workOrder );
							_= semaphore.Release();
						},
						token
					) );
				}
				System.Threading.Tasks.Task.WaitAll( tasks.ToArray(), token );
			}
		}
		#endregion methods

	}

}
