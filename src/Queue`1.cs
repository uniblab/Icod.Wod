namespace Icod.Wod {

	[System.Serializable]
	public sealed class Queue<T> : IQueue<T> {

		#region nested classes
		internal sealed class EmptyQueue<U> : IQueue<U> {
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

			public IQueue<U> Dequeue() {
				throw new System.InvalidOperationException();
			}
			public U Peek() {
				throw new System.InvalidOperationException();
			}
			public IQueue<U> Enqueue( U value ) {
				return new SingleQueue<U>( value );
			}
			public IQueue<U> Reverse() {
				throw new System.InvalidOperationException();
			}
			public IStack<U> ToStack() {
				return Stack<U>.Empty;
			}
			public System.Collections.Generic.IEnumerator<U> GetEnumerator() {
				yield break;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield break;
			}
		}

		internal sealed class SingleQueue<U> : IQueue<U> {
			private readonly U myValue;

			internal SingleQueue() : base() {
			}
			internal SingleQueue( U value ) : this() {
				myValue = value;
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

			public IQueue<U> Dequeue() {
				return Queue<U>.Empty;
			}
			public U Peek() {
				return myValue;
			}
			public IQueue<U> Enqueue( U value ) {
				return new Queue<U>( Stack<U>.Empty.Push( myValue ), Stack<U>.Empty.Push( value ) );
			}
			public IQueue<U> Reverse() {
				return this;
			}
			public IStack<U> ToStack() {
				return Stack<U>.Empty.Push( myValue );
			}
			public System.Collections.Generic.IEnumerator<U> GetEnumerator() {
				yield return myValue;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield return myValue;
			}
		}
		#endregion nested classes


		#region fields
		private static readonly IQueue<T> theEmpty;

		private readonly System.Int32 myCount;
		private readonly IStack<T> myDrain;
		private readonly IStack<T> mySource;
		#endregion fields


		#region .ctor
		static Queue() {
			theEmpty = new EmptyQueue<T>();
		}

		internal Queue( IStack<T> drain, IStack<T> source ) {
			source = source ?? Stack<T>.Empty;
			if ( ( drain ?? Stack<T>.Empty ).IsEmpty ) {
				drain = source.Reverse();
				source = Stack<T>.Empty;
			}
			mySource = source;
			myDrain = drain;
			myCount = mySource.Count + myDrain.Count;
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