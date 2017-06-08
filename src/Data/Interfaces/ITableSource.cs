using System.Linq;

namespace Icod.Wod.Data {

	public interface ITableSource {

		System.Collections.Generic.IEnumerable<System.Data.DataTable> ReadTables( Icod.Wod.WorkOrder order );

	}

}