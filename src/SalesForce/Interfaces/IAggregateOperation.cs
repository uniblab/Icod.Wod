// Copyright 2020, Timothy J. Bruce
namespace Icod.Wod.SalesForce.Bulk {

	public interface IAggregateOperation {

		void PerformWork( Pair<LoginResponse, IStep> jobProcess );

	}

}
