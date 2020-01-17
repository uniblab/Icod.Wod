namespace Icod.Wod.SalesForce.Bulk {

	public interface IAggregateOperation {

		void PerformWork( JobProcess jobProcess );

	}

}