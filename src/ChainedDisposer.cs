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

using System.Linq;

namespace Icod.Wod {

	[System.Xml.Serialization.XmlType( IncludeInSchema = false )]
	public sealed class ChainedDisposer : System.IDisposable {

		#region fields
		private readonly System.IDisposable myOuter;
		private readonly System.IDisposable myInner;
		private System.Boolean myIsDisposed;
		#endregion fields


		#region .ctor
		public ChainedDisposer( System.IDisposable outer, System.IDisposable inner ) : base() {
			myIsDisposed = false;
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
				if ( !System.Threading.Volatile.Read( ref myIsDisposed ) ) {
					if ( myInner is object ) {
						myInner.Dispose();
					}
					if ( myOuter is object ) {
						myOuter.Dispose();
					}
					System.Threading.Volatile.Write( ref myIsDisposed, true );
				}
				System.Threading.Thread.EndCriticalRegion();
			}
		}
		#endregion methods

	}

}
