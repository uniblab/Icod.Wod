using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"dbCommand",
		Namespace = "http://Icod.Wod"
	)]
	public class DbCommand : DbDescriptor, Icod.Wod.IStep {

		#region .ctor
		public DbCommand() : base() {
		}
		#endregion .ctor


		#region methods
		public void DoWork( Icod.Wod.WorkOrder order ) {
			using ( var cnxn = this.CreateConnection( order ) ) {
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