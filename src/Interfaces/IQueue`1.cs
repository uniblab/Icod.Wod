﻿namespace Icod.Wod {

	public interface IQueue<T> : System.Collections.Generic.IEnumerable<T>, IIsEmpty {

		T Peek();

		IQueue<T> Enqueue( T value );
		IQueue<T> Dequeue();

		System.Int32 Count {
			get;
		}

		IQueue<T> Reverse();

	}

}