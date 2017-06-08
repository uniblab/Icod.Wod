using System.Linq;

namespace Icod.Wod.Data {

	[System.Serializable]
	[System.Xml.Serialization.XmlInclude( typeof( FileImport ) )]
	[System.Xml.Serialization.XmlType(
		"dbOperation",
		Namespace = "http://Icod.Wod"
	)]
	public abstract class DbOperationBase : IStep {

		#region .ctor
		protected DbOperationBase() : base() {
		}
		#endregion .ctor


		#region methods
		public abstract void DoWork( WorkOrder order );
		#endregion methods

	}

}