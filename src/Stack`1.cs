using System.Linq;

namespace Icod.Wod {

	[System.Serializable]
	public sealed class Stack<T> : IStack<T> {

		#region nested classes
		internal sealed class EmptyStack<T> : IStack<T> {
			private static readonly System.Int32 theHashCode;
			static EmptyStack() {
				unchecked {
					theHashCode = typeof( EmptyStack<T> ).AssemblyQualifiedName.GetHashCode() + typeof( T ).AssemblyQualifiedName.GetHashCode();
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
				return new SingleStack<T>( value );
			}
			public IStack<T> Reverse() {
				return this;
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
		internal sealed class SingleStack<T> : IStack<T> {
			private static readonly System.Int32 theHashCode;
			private readonly T myValue;
			private readonly System.Int32 myHashCode;
			static SingleStack() {
				unchecked {
					theHashCode = typeof( SingleStack<T> ).AssemblyQualifiedName.GetHashCode() + typeof( T ).AssemblyQualifiedName.GetHashCode();
				}
			}
			internal SingleStack( T value ) : base() {
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
			public sealed override System.Int32 GetHashCode() {
				return theHashCode;
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
		#endregion methods

	}

}