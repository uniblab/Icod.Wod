// Copyright (C) 2023  Timothy J. Bruce
namespace Icod.Wod {

	public interface IStack<T> : System.Collections.Generic.IEnumerable<T>, IIsEmpty {

		T Peek();
		IStack<T> Push( T value );
		IStack<T> Pop();

		System.Int32 Count {
			get;
		}

		IStack<T> Reverse();

		IQueue<T> ToQueue();

	}

}
