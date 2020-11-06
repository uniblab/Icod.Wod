namespace Icod.Wod {

	public interface IStep {


		WorkOrder WorkOrder {
			get;
		}

		void DoWork( WorkOrder workOrder );

	}

}
