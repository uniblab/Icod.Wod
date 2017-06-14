using System.Linq;

namespace Icod.Wod.File {

	internal sealed class ChainedDisposer : System.IDisposable {

		#region fields
		private System.IDisposable myFirst;
		private System.IDisposable mySecond;
		#endregion fields


		#region .ctor
		private ChainedDisposer() : base() {
		}
		public ChainedDisposer( System.IDisposable first, System.IDisposable second ) : this() {
			mySecond = second;
			myFirst = first;
		}

		~ChainedDisposer() {
			this.Dispose( false );
		}
		#endregion .ctor


		#region methods
		public void Dispose() {
			this.Dispose( true );
			System.GC.SuppressFinalize( this );
		}
		private void Dispose( System.Boolean disposing ) {
			if ( disposing ) {
				if ( null != mySecond ) {
					mySecond.Dispose();
					mySecond = null;
				}
				if ( null != myFirst ) {
					myFirst.Dispose();
					myFirst = null;
				}
			}
		}
		#endregion methods

	}

}