// Copyright 2023, Timothy J. Bruce
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
			var loginResponse = new Login() { WorkOrder = workOrder }.GetLoginResponse( this.InstanceName );
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
