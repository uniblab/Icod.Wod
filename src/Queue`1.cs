namespace Icod.Wod {

	[System.Serializable]
	public sealed class Queue<T> : IQueue<T> {

		#region nested classes
		internal sealed class EmptyQueue : IQueue<T> {
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
				return new Queue<T>( Stack<T>.Empty.Push( value ), Stack<T>.Empty );
			}
			public IQueue<T> Reverse() {
				throw new System.InvalidOperationException();
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield break;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield break;
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
			theEmpty = new EmptyQueue();
		}

		internal Queue( IStack<T> drain, IStack<T> source ) {
			if ( drain.IsEmpty ) {
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
			throw new System.InvalidOperationException();
		}
		public IQueue<T> Reverse() {
			return new Queue<T>( mySource.Reverse(), myDrain.Reverse() );
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