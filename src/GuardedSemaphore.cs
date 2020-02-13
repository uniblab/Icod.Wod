namespace Icod.Wod {

	public sealed class GuardedSemaphore {

		#region fields
		private readonly Icod.Wod.Semaphore mySemaphore;
		#endregion fields


		#region .ctor
		public GuardedSemaphore( Icod.Wod.Semaphore semaphore ) : base() {
			if ( null == semaphore ) {
				throw new System.ArgumentNullException( "semaphore" );
			}
			mySemaphore = semaphore;
		}
		#endregion .ctor


		#region methods
		public System.Int32 Release() {
			return mySemaphore.Release();
		}
		public void Wait() {
			mySemaphore.Wait();
		}
		public System.Boolean Wait( System.TimeSpan timeout ) {
			return mySemaphore.Wait( timeout );
		}
		public System.Boolean Wait( System.Int32 millisecondsTimeout ) {
			return mySemaphore.Wait( millisecondsTimeout );
		}
		#endregion methods

	}

}