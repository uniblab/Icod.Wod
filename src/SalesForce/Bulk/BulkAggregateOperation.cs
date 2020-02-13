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
		public const System.Int32 DefaultMaxDegreeOfParallelism = 4;
		private System.String myInstanceName;
		#endregion fields


		#region .ctor
		public BulkAggregateOperation() : base() {
			myInstanceName = null;
			this.MaxDegreeOfParallelism = DefaultMaxDegreeOfParallelism;
		}
		public BulkAggregateOperation( WorkOrder workOrder ) : base( workOrder ) {
			myInstanceName = null;
			this.MaxDegreeOfParallelism = DefaultMaxDegreeOfParallelism;
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
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var operations = ( this.Operations ?? new System.Object[ 0 ] ).OfType<IAggregateOperation>();
			if ( !operations.Any() ) {
				return;
			}
			var cred = Credential.GetCredential( this.InstanceName, workOrder );
			var name = cred.ClientId + ( cred.Username ?? cred.RefreshToken ?? System.String.Empty );
			using ( var semaphore = new Icod.Wod.Semaphore( this.MaxDegreeOfParallelism, this.MaxDegreeOfParallelism, name ) ) {
				semaphore.Wait();
				var loginResponse = new Login( workOrder ).GetLoginResponse( this.InstanceName );
				semaphore.Release();
				var jobProcess = new JobProcess( loginResponse, this, semaphore );
				var threads = new System.Collections.Generic.List<System.Threading.Thread>();
				System.Threading.Thread thread = null;
				foreach ( var operation in operations ) {
					thread = new System.Threading.Thread( x => operation.PerformWork( x as JobProcess ) );
					threads.Add( thread );
					thread.Start( jobProcess );
				}
				foreach ( var t in threads ) {
					t.Join();
				}
			}
		}
		#endregion methods

	}

}