using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"command",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class Command : DbOperationBase {

		#region .ctor
		public Command() : base() {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( Icod.Wod.WorkOrder workOrder, IStack<ContextRecord> context ) {
			using ( var cnxn = this.CreateConnection( workOrder ) ) {
				if ( System.Data.ConnectionState.Open != cnxn.State ) {
					cnxn.Open();
				}
				using ( var cmd = this.CreateCommand( cnxn ) ) {
					cmd.ExecuteNonQuery();
				}
			}
		}
		#endregion methods

	}

}