// Icod.Wod.dll is the Work on Demand framework.
// Copyright (C) 2023  Timothy J. Bruce

/*
    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
	version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
    USA
*/

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
				if ( mySemaphore is object ) {
					mySemaphore.Dispose();
				}
				if ( mySemaphoreSlim is object ) {
					mySemaphoreSlim.Dispose();
				}
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
