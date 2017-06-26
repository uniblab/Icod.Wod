using System.Linq;

namespace Icod.Wod.File {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public sealed class ChainedDisposer : System.IDisposable {

		#region fields
		private System.IDisposable myOuter;
		private System.IDisposable myInner;
		#endregion fields


		#region .ctor
		private ChainedDisposer() : base() {
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
			if ( disposing ) {
				if ( null != myInner ) {
					myInner.Dispose();
					myInner = null;
				}
				if ( null != myOuter ) {
					myOuter.Dispose();
					myOuter = null;
				}
			}
		}
		#endregion methods

	}

}