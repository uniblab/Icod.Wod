// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod.SalesForce.Bulk {

	public sealed class JobProcess {

		#region fields
		private readonly LoginResponse myLoginResponse;
		private readonly IStep myStep;
		private readonly Icod.Wod.Semaphore mySemaphore;
		#endregion fields


		#region .ctor
		public JobProcess( LoginResponse loginResponse, IStep step, Icod.Wod.Semaphore semaphore ) : base() {
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
		public Icod.Wod.Semaphore Semaphore {
			get {
				return mySemaphore;
			}
		}
		#endregion properties


	}

}
