// Copyright 2023, Timothy J. Bruce
namespace Icod.Wod {

	public sealed class Semaphore : System.IDisposable {

		#region fields
		public const System.Int32 Unlimited = -1;
		private readonly System.Threading.Semaphore mySemaphore;
		private readonly System.Threading.SemaphoreSlim mySemaphoreSlim;

		private readonly System.Action myWait;
		private readonly System.Func<System.TimeSpan, System.Boolean> myWaitTimeSpan;
		private readonly System.Func<System.Int32, System.Boolean> myWaitMilliseconds;
		private readonly System.Func<System.Int32> myRelease;
		#endregion fields


		#region .ctor
		public Semaphore( System.Int32 initialCount, System.Int32 maximumCount ) : this( initialCount, maximumCount, null ) {
		}
		public Semaphore( System.Int32 initialCount, System.Int32 maximumCount, System.String name ) : base() {
			if ( Unlimited == maximumCount ) {
				myRelease = () => 1;
				myWait = () => {
					;
				};
				myWaitTimeSpan = timeout => true;
				myWaitMilliseconds = millisecondsTimeout => true;
			} else if ( System.String.IsNullOrEmpty( name ) ) {
				mySemaphoreSlim = new System.Threading.SemaphoreSlim( initialCount, maximumCount );
				myRelease = () => mySemaphoreSlim.Release();
				myWait = () => mySemaphoreSlim.Wait();
				myWaitTimeSpan = timeout => mySemaphoreSlim.Wait( timeout );
				myWaitMilliseconds = millisecondsTimeout => mySemaphoreSlim.Wait( millisecondsTimeout );
			} else {
				mySemaphore = new System.Threading.Semaphore( initialCount, maximumCount, name );
				myRelease = () => mySemaphore.Release();
				myWait = () => mySemaphore.WaitOne();
				myWaitTimeSpan = timeout => mySemaphore.WaitOne( timeout );
				myWaitMilliseconds = millisecondsTimeout => mySemaphore.WaitOne( millisecondsTimeout );
			}
		}

		~Semaphore() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region methods
		public void Dispose() {
			this.Dispose( true );
			System.GC.SuppressFinalize( this );
		}
		protected void Dispose( System.Boolean disposing ) {
			if ( disposing ) {
				mySemaphore?.Dispose();
				mySemaphoreSlim?.Dispose();
			}
		}

		public System.Int32 Release() {
			return myRelease();
		}
		public void Wait() {
			myWait();
		}
		public System.Boolean Wait( System.TimeSpan timeout ) {
			return myWaitTimeSpan( timeout );
		}
		public System.Boolean Wait( System.Int32 millisecondsTimeout ) {
			return myWaitMilliseconds( millisecondsTimeout );
		}
		#endregion methods

	}

}
