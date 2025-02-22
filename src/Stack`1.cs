// Copyright (C) 2025  Timothy J. Bruce

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Stack<T> : IStack<T> {

		#region nested classes
		internal sealed class EmptyStack : IStack<T> {
			private static readonly System.Int32 theHashCode;
			static EmptyStack() {
				theHashCode = typeof( EmptyStack ).AssemblyQualifiedName.GetHashCode();
				unchecked {
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
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
			public T Peek() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Pop() {
				throw new System.InvalidOperationException();
			}
			public IStack<T> Push( T value ) {
				return new SingleStack( value );
			}
			public IStack<T> Reverse() {
				return this;
			}
			public IQueue<T> ToQueue() {
				return Queue<T>.Empty;
			}

			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
			}
			public sealed override System.Boolean Equals( System.Object obj ) {
				return ReferenceEquals( this, obj );
			}
			public System.Collections.Generic.IEnumerator<T> GetEnumerator() {
				yield break;
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				yield break;
			}
		}
		internal sealed class SingleStack : IStack<T> {
			private static readonly System.Int32 theHashCode;
			private readonly T myValue;
			private readonly System.Int32 myHashCode;
			static SingleStack() {
				theHashCode = typeof( SingleStack ).AssemblyQualifiedName.GetHashCode();
				unchecked {
					theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			private SingleStack() : base() {
				myHashCode = theHashCode;
			}
			internal SingleStack( T value ) : this() {
				myValue = value;
				if ( value is object ) {
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
			public T Peek() {
				return myValue;
			}
			public IStack<T> Pop() {
				return Stack<T>.Empty;
			}
			public IStack<T> Push( T value ) {
				return new Stack<T>( this, value );
			}
			public IStack<T> Reverse() {
				return this;
			}
			public IQueue<T> ToQueue() {
				return Queue<T>.Empty.Enqueue( myValue );
			}

			public sealed override System.Int32 GetHashCode() {
				return myHashCode;
			}
			public sealed override System.Boolean Equals( System.Object obj ) {
				return ReferenceEquals( this, obj );
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
		private static readonly IStack<T> theEmpty;

		private readonly T myValue;
		private readonly IStack<T> myTail;
		private readonly System.Int32 myCount;
		private readonly System.Int32 myHashCode;
		#endregion fields


		#region .ctor
		static Stack() {
			theEmpty = new EmptyStack();
			theHashCode = typeof( Stack<T> ).AssemblyQualifiedName.GetHashCode();
			unchecked {
				theHashCode += typeof( T ).AssemblyQualifiedName.GetHashCode();
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
				if ( value is object ) {
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

		public sealed override System.Int32 GetHashCode() {
			return myHashCode;
		}
		public sealed override System.Boolean Equals( System.Object obj ) {
			return ReferenceEquals( this, obj );
		}
		#endregion methods

	}

}
