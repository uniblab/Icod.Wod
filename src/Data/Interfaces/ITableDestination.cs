using System.Linq;

namespace Icod.Wod.Data {

	public interface ITableDestination {

		void WriteRecords( Icod.Wod.WorkOrder order, ITableSource source );

	}

}