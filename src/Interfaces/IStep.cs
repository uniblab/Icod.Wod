// Copyright 2023, Timothy J. Bruce
namespace Icod.Wod {

	public interface IStep {


		WorkOrder WorkOrder {
			get;
		}

		void DoWork( WorkOrder workOrder );

	}

}
