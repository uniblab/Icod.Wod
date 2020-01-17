using System.Linq;

namespace Icod.Wod.SalesForce.Bulk {

	public sealed class JobProcess {

		#region fields
		private readonly LoginResponse myLoginResponse;
		private readonly IStep myStep;
		private readonly GuardedSemaphore mySemaphore;
		#endregion fields


		#region .ctor
		public JobProcess( LoginResponse loginResponse, IStep step, GuardedSemaphore semaphore ) : base() {
			myLoginResponse = loginResponse;
			myStep = step;
			mySemaphore = semaphore;
		}
		#endregion .ctor


		#region properties
		public LoginResponse LoginResponse {
			get {
				return myLoginResponse;
			}
		}
		public IStep Step {
			get {
				return myStep;
			}
		}
		public GuardedSemaphore Semaphore {
			get {
				return mySemaphore;
			}
		}
		#endregion properties


	}

}