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

	[System.Serializable]
	public sealed class Queue<T> : IQueue<T> {

		#region nested classes
		internal sealed class EmptyQueue : IQueue<T> {
			private static readonly System.Int32 theHashCode;
			static EmptyQueue() {
				theHashCode = typeof( EmptyQueue ).AssemblyQualifiedName!.GetHashCode();
				unchecked {
					theHashCode += typeof( T ).AssemblyQualifiedName!.GetHashCode();
				}
			}

			internal EmptyQueue() : base() {
			}

			public System.Int32 Count {
				get {
					return 0;
				}
			}
			public System.Boolean IsEmpty {
				get {
					return true;
				}
			}

			public IQueue<T> Dequeue() {
				throw new System.InvalidOperationException();
			}
			public T Peek() {
				throw new System.InvalidOperationException();
			}
			public IQueue<T> Enqueue( T value ) {
				return new SingleQueue( value );
			}
			public IQueue<T> Reverse() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> ToStack() {
				return Stack<T>.Empty;
			}
			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield break;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield break;
			}
		}

		internal sealed class SingleQueue : IQueue<T> {
			private static readonly System.Int32 theHashCode;
			private readonly T myValue;
			private readonly System.Int32 myHashCode;

			static SingleQueue() {
				theHashCode = typeof( SingleQueue ).AssemblyQualifiedName!.GetHashCode();
				unchecked {
					theHashCode += typeof( T ).AssemblyQualifiedName!.GetHashCode();
				}
			}
			internal SingleQueue( T value ) : base() {
				myValue = value;
				if ( value is object ) {
					unchecked {
						myHashCode = theHashCode + value.GetHashCode();
					}
				}
			}

			public System.Int32 Count {
				get {
					return 1;
				}
			}
			public System.Boolean IsEmpty {
				get {
					return false;
				}
			}

			public IQueue<T> Dequeue() {
				return Queue<T>.Empty;
			}
			public T Peek() {
				return myValue;
			}
			public IQueue<T> Enqueue( T value ) {
				return new Queue<T>( Stack<T>.Empty.Push( myValue ), Stack<T>.Empty.Push( value ) );
			}
			public IQueue<T> Reverse() {
				return this;
			}
			public IStack<T> ToStack() {
				return Stack<T>.Empty.Push( myValue );
			}
			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield return myValue;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield return myValue;
			}
		}
		#endregion nested classes


		#region fields
		private static readonly System.Int32 theHashCode;
		private static readonly IQueue<T> theEmpty;

		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		private readonly IStack<T> myDrain;
		private readonly IStack<T> mySource;
		#endregion fields


		#region .ctor
		static Queue() {
			theEmpty = new EmptyQueue();
			theHashCode = typeof( Queue<T> ).AssemblyQualifiedName!.GetHashCode();
			unchecked {
				theHashCode += typeof( T ).AssemblyQualifiedName!.GetHashCode();
			}
		}

		internal Queue( IStack<T> drain, IStack<T> source ) : base() {
			source = source ?? Stack<T>.Empty;
			if ( ( drain ?? Stack<T>.Empty ).IsEmpty ) {
				drain = source.Reverse();
				source = Stack<T>.Empty;
			}
			mySource = source;
			myDrain = drain!;
			myCount = mySource.Count + myDrain!.Count;
			unchecked {
				myHashCode = theHashCode;
				myHashCode += ( drain ?? Stack<T>.Empty ).GetHashCode();
				myHashCode += ( source ?? Stack<T>.Empty ).GetHashCode();
			}
		}
		#endregion .ctor


		#region properties
		public static IQueue<T> Empty {
			get {
				return theEmpty;
			}
		}

		public System.Int32 Count {
			get {
				return myCount;
			}
		}
		public System.Boolean IsEmpty {
			get {
				return false;
			}
		}
		#endregion properties


		#region methods
		public IQueue<T> Enqueue( T value ) {
			return new Queue<T>( myDrain, mySource.Push( value ) );
		}
		public T Peek() {
			return myDrain.Peek();
		}
		public IQueue<T> Dequeue() {
			var d = myDrain.Pop();
			if ( !d.IsEmpty ) {
				return new Queue<T>( d, mySource );
			} else if ( mySource.IsEmpty ) {
				return Queue<T>.Empty;
			} else {
				return new Queue<T>( mySource.Reverse(), Stack<T>.Empty );
			}
		}
		public IQueue<T> Reverse() {
			return new Queue<T>( mySource.Reverse(), myDrain.Reverse() );
		}
		public IStack<T> ToStack() {
			var output = Stack<T>.Empty;
			foreach ( var item in this ) {
				output = output.Push( item );
			}
			return output;
		}


		public sealed override System.Int32 GetHashCode() {
			return myHashCode;
		}
		public sealed override System.Boolean Equals( System.Object? obj ) {
			if ( obj is null ) {
				return false;
			} else if ( ReferenceEquals( this, obj ) ) {
				return true;
			} else if ( obj is not Queue<T> ) {
				return false;
			}
			var other = (IQueue<T>)obj;
			if ( this.Count != other.Count ) {
				return false;
			} else if ( myHashCode != other.GetHashCode() ) {
				return false;
			}
			IQueue<T> probe = this;
			while ( !probe.IsEmpty ) {
				if ( !Equals( probe.Peek(), other.Peek() ) ) {
					return false;
				}
				probe = probe.Dequeue();
				other = other.Dequeue();
			}
			return true;
		}
		public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
			IQueue<T> probe = this;
			while ( !probe.IsEmpty ) {
				yield return probe.Peek();
				probe = probe.Dequeue();
			}
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
		#endregion methods

	}

}
