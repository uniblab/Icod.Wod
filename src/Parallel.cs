using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	[System.Xml.Serialization.XmlType(
		"parallel",
		Namespace = "http://Icod.Wod"
	)]
	public sealed class Parallel : StepContainerBase, IStep {

		#region .ctor
		public Parallel() : base() {
		}
		#endregion .ctor


		#region methods
		public void DoWork( Icod.Wod.WorkOrder workOrder ) {
			System.Threading.Tasks.Parallel.Invoke( ( this.Steps ?? new IStep[ 0 ] ).OfType<IStep>().Select<IStep, System.Action>(
				x => () => x.DoWork( workOrder )
			).ToArray() );
		}
		#endregion methods

	}

}