namespace Icod.Wod {

	public sealed class GuardedSemaphore {

		#region fields
		private readonly System.Threading.SemaphoreSlim mySemaphore;
		#endregion fields


		#region .ctor
		public GuardedSemaphore( System.Threading.SemaphoreSlim semaphore ) : base() {
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
		public System.Boolean Wait( System.TimeSpan timeout, System.Threading.CancellationToken cancellationToken ) {
			return mySemaphore.Wait( timeout, cancellationToken );
		}
		public System.Boolean Wait( System.Int32 millisecondsTimeout ) {
			return mySemaphore.Wait( millisecondsTimeout );
		}
		public System.Boolean Wait( System.Int32 millisecondsTimeout, System.Threading.CancellationToken cancellationToken ) {
			return mySemaphore.Wait( millisecondsTimeout, cancellationToken );
		}
		#endregion methods

	}

}