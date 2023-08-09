// Copyright 2023, Timothy J. Bruce
using System.Linq;

namespace Icod.Wod.Data {

	public interface ITableDestination {

		void WriteRecords( Icod.Wod.WorkOrder workOrder, ITableSource source );

	}

}
