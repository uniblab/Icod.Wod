using System.Linq;

namespace Icod.Wod.File {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"chopFile",
		Namespace = "http://Icod.Wod",
		IncludeInSchema = true
	)]
	public sealed class ChopFile : CountBinaryFileOperationBase {

		#region .ctor
		public ChopFile() : base() {
		}
		public ChopFile( WorkOrder workOrder ) : base( workOrder ) {
		}
		#endregion .ctor


		#region methods
		public sealed override void DoWork( WorkOrder workOrder, IStack<ContextRecord> context ) {
			this.Context = context ?? Stack<ContextRecord>.Empty;
			this.WorkOrder = workOrder ?? throw new System.ArgumentNullException( "workOrder" );
			var count = this.Count;
			if ( count <= 0 ) {
				throw new System.InvalidOperationException( "Count must be positive." );
			}
			var worker = new TailFile( workOrder );
			workOrder
		}
		#endregion methods


	}

}