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

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"bulkAggregateOperation",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class BulkAggregateOperation : SFOperationBase, IStep {

		#region fields
		private System.String myInstanceName;
		#endregion fields


		#region .ctor
		public BulkAggregateOperation() : base() {
			myInstanceName = null;
		}
		public BulkAggregateOperation( WorkOrder workOrder ) : base( workOrder ) {
			myInstanceName = null;		}
		#endregion .ctor


		#region properties
		[System.Xml.Serialization.XmlAttribute(
			"instanceName",
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( (System.String)null )]
		public System.String InstanceName {
			get {
				return myInstanceName;
			}
			set {
				myInstanceName = value.TrimToNull();
			}
		}

		[System.Xml.Serialization.XmlArray(
			IsNullable = false,
			Namespace = "http://Icod.Wod",
			ElementName = "operations"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.Query ),
			ElementName = "query",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.Delete ),
			ElementName = "delete",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.Insert ),
			ElementName = "insert",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.Update ),
			ElementName = "update",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.Xml.Serialization.XmlArrayItem(
			typeof( SalesForce.Bulk.Upsert ),
			ElementName = "upsert",
			IsNullable = false,
			Namespace = "http://Icod.Wod"
		)]
		[System.ComponentModel.DefaultValue( null )]
		public AggregateOperationBase[] Operations {
			get;
			set;
		}
		#endregion properties


		#region methods
		public void DoWork( WorkOrder workOrder ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var operations = ( this.Operations ?? new System.Object[ 0 ] ).OfType<IAggregateOperation>();
			if ( !operations.Any() ) {
				return;
			}
			var cred = Credential.GetCredential( this.InstanceName, workOrder );
			var name = cred.ClientId + ( cred.Username ?? cred.RefreshToken ?? System.String.Empty );
			var loginResponse = new Login( workOrder ).GetLoginResponse( this.InstanceName );
			var jobProcess = new Pair<LoginResponse, IStep>( loginResponse, this );

			using ( var tokenSource = new System.Threading.CancellationTokenSource() ) {
				var token = tokenSource.Token;
				System.Collections.Generic.ICollection<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
				var factory = new System.Threading.Tasks.TaskFactory(
					token,
					System.Threading.Tasks.TaskCreationOptions.LongRunning,
					System.Threading.Tasks.TaskContinuationOptions.LongRunning,
					System.Threading.Tasks.TaskScheduler.Default
				);
				foreach ( var operation in operations ) {
					tasks.Add( factory.StartNew( () => operation.PerformWork( jobProcess ), token ) );
				}
				System.Threading.Tasks.Task.WaitAll( tasks.ToArray(), token );
			}
		}
		#endregion methods

	}

}
