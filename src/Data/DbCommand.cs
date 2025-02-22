// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		TypeName = "dbCommand",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class DbCommand : DbOperationBase, Icod.Wod.IStep {

		#region .ctor
		public DbCommand() : base() {
		}
		#endregion .ctor


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			using ( var cnxn = this.CreateConnection( workOrder ) ) {
				if ( System.Data.ConnectionState.Open != cnxn.State ) {
					cnxn.Open();
				}
				using ( var cmd = this.CreateCommand( cnxn ) ) {
					_ = cmd.ExecuteNonQuery();
				}
			}
		}
		#endregion methods

	}

}
