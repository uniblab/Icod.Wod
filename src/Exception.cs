using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	public class Exception : System.ApplicationException {

		#region fields
		private readonly WorkOrder myWorkOrder;
		private readonly IStep myStep;
		private readonly System.Int32 myStepNumber;
		#endregion fields


		#region .ctor
		protected Exception() : base() {
			myWorkOrder = null;
			myStep = null;
			myStepNumber = -1;
		}
		protected Exception( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context ) : base( info, context ) {
			myWorkOrder = null;
			myStep = null;
			myStepNumber = -1;
		}

		public Exception( System.String message ) : base( message ) {
			myWorkOrder = null;
			myStep = null;
			myStepNumber = -1;
		}
		public Exception( System.String message, System.Int32 stepNumber, WorkOrder workOrder, IStep step ) : this( message ) {
			myStepNumber = stepNumber;
			this.Data.Add( "stepNumber", myStepNumber );
			myWorkOrder = workOrder;
			this.Data.Add( "workOrder", myWorkOrder );
			myStep = step;
			this.Data.Add( "step", myStep );
		}
		public Exception( System.String message, System.Exception innerException ) : base( message, innerException ) {
			myWorkOrder = null;
			myStep = null;
			myStepNumber = -1;
		}
		public Exception( System.String message, System.Exception innerException, System.Int32 stepNumber, WorkOrder workOrder, IStep step ) : this( message, innerException ) {
			myStepNumber = stepNumber;
			this.Data.Add( "stepNumber", myStepNumber );
			myWorkOrder = workOrder;
			this.Data.Add( "workOrder", myWorkOrder );
			myStep = step;
			this.Data.Add( "step", myStep );
		}
		#endregion .ctor


		#region properties
		public IStep Step {
			get {
				return myStep;
			}
		}
		public System.Int32 StepNumber {
			get {
				return myStepNumber;
			}
		}
		public WorkOrder WorkOrder {
			get {
				return myWorkOrder;
			}
		}
		#endregion properties

	}

}