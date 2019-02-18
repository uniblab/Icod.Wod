using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Stack<T> : IStack<T> {

		#region nested classes
		internal sealed class EmptyStack<U> : IStack<U> {
			private static readonly System.Int32 theHashCode;
			static EmptyStack() {
				unchecked {
					theHashCode = typeof( EmptyStack<U> ).AssemblyQualifiedName.GetHashCode() + typeof( U ).AssemblyQualifiedName.GetHashCode();
				}
			}
			internal EmptyStack() : base() {
			}
			public System.Boolean IsEmpty {
				get {
					return true;
				}
			}
			public System.Int32 Count {
				get {
					return 0;
				}
			}
			public U Peek() {
				throw new System.InvalidOperationException();
			}
			public IStack<U> Pop() {
				throw new System.InvalidOperationException();
			}
			public IStack<U> Push( U value ) {
				return new SingleStack<U>( value );
			}
			public IStack<U> Reverse() {
				return this;
			}
			public IQueue<U> ToQueue() {
				return Queue<U>.Empty;
			}

			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
			}
			public System.Collections.Generic.IEnumerator<U> GetEnumerator() {
				yield break;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield break;
			}
		}
		internal sealed class SingleStack<U> : IStack<U> {
			private static readonly System.Int32 theHashCode;
			private readonly U myValue;
			private readonly System.Int32 myHashCode;
			static SingleStack() {
				unchecked {
					theHashCode = typeof( SingleStack<U> ).AssemblyQualifiedName.GetHashCode() + typeof( U ).AssemblyQualifiedName.GetHashCode();
				}
			}
			internal SingleStack( U value ) : base() {
				myValue = value;
				myHashCode = theHashCode;
				if ( !System.Object.ReferenceEquals( value, null ) ) {
					unchecked {
						myHashCode += value.GetHashCode();
					}
				}
			}
			public System.Boolean IsEmpty {
				get {
					return false;
				}
			}
			public System.Int32 Count {
				get {
					return 1;
				}
			}
			public U Peek() {
				return myValue;
			}
			public IStack<U> Pop() {
				return Stack<U>.Empty;
			}
			public IStack<U> Push( U value ) {
				return new Stack<U>( this, value );
			}
			public IStack<U> Reverse() {
				return this;
			}
			public IQueue<U> ToQueue() {
				return Queue<U>.Empty.Enqueue( myValue );
			}

			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
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
		private static readonly System.Int32 theHashCode;
		private static readonly IStack<T> theEmpty;

		private readonly T myValue;
		private readonly IStack<T> myTail;
		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		#endregion fields


		#region .ctor
		static Stack() {
			theEmpty = new EmptyStack<T>();
			unchecked {
				theHashCode = typeof( SingleStack<T> ).AssemblyQualifiedName.GetHashCode() + typeof( T ).AssemblyQualifiedName.GetHashCode();
			}
		}

		private Stack() : base() {
			myHashCode = theHashCode;
		}
		private Stack( IStack<T> tail, T value ) : this() {
			myValue = value;
			myTail = tail ?? Stack<T>.Empty;
			myCount = 1 + myTail.Count;
			unchecked {
				myHashCode += myTail.GetHashCode();
				if ( !System.Object.ReferenceEquals( value, null ) ) {
					myHashCode += value.GetHashCode();
				}
			}
		}
		#endregion .ctor


		#region properties
		public static IStack<T> Empty {
			get {
				return theEmpty;
			}
		}

		public System.Boolean IsEmpty {
			get {
				return false;
			}
		}
		public System.Int32 Count {
			get {
				return myCount;
			}
		}
		#endregion properties


		#region methods
		public T Peek() {
			return myValue;
		}
		public IStack<T> Push( T value ) {
			return new Stack<T>( this, value );
		}
		public IStack<T> Pop() {
			return myTail;
		}

		public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
			IStack<T> probe = this;
			while ( !probe.IsEmpty ) {
				yield return probe.Peek();
				probe = probe.Pop();
			}
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public IStack<T> Reverse() {
			var output = Stack<T>.Empty;
			IStack<T> probe = this;
			while ( !probe.IsEmpty ) {
				output = output.Push( probe.Peek() );
				probe = probe.Pop();
			}
			return output;
		}

		public IQueue<T> ToQueue() {
			var output = Queue<T>.Empty;
			foreach ( var item in this.Reverse() ) {
				output = output.Enqueue( item );
			}
			return output;
		}
		#endregion methods

	}

}