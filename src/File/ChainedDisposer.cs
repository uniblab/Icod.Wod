using System.Linq;

namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public sealed class ChainedDisposer : System.IDisposable {

		#region fields
		private readonly System.IDisposable myOuter;
		private readonly System.IDisposable myInner;
		private System.Boolean myIsDisposed;
		#endregion fields


		#region .ctor
		private ChainedDisposer() : base() {
			myIsDisposed = false;
		}
		public ChainedDisposer( System.IDisposable outer, System.IDisposable inner ) : this() {
			myInner = inner;
			myOuter = outer;
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
			if ( disposing && !myIsDisposed ) {
				System.Threading.Thread.BeginCriticalRegion();
				if ( null != myInner ) {
					myInner.Dispose();
				}
				if ( null != myOuter ) {
					myOuter.Dispose();
				}
				myIsDisposed = true;
				System.Threading.Thread.EndCriticalRegion();
			}
		}
		#endregion methods

	}

}