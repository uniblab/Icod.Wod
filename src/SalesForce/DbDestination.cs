using System.Linq;

namespace Icod.Wod.SalesForce {

	[System.Serializable]
	public sealed class DbDestination : Icod.Wod.Data.DbIODescriptorBase {

		#region .ctor
		public DbDestination() : base() {
		}
		public DbDestination( WorkOrder workOrder ) : this() {
			this.WorkOrder = workOrder;
		}
		#endregion .ctor


		#region methods
		public sealed override void WriteRecords( Icod.Wod.WorkOrder workOrder, Icod.Wod.Data.ITableSource source ) {
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			if ( System.String.IsNullOrEmpty( this.CommandText ) ) {
				base.WriteRecords( workOrder, source );
			} else {
				this.ExecuteCommand( workOrder, source );
			}
		}
		#endregion methods

	}

}