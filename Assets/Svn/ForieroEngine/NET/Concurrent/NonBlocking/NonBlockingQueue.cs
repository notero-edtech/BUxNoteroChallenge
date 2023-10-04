using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ForieroEngine.Collections.NonBlocking
{
	public class NonBlockingQueue<T>
	{
		class NoteT
		{
			public T value;
			public PointerT next;
			/// <summary>
			/// default constructor
			/// </summary>
			public NoteT() { }
		}

		struct PointerT
		{
			public long count;
			public NoteT ptr;

			/// <summary>
			/// copy constructor
			/// </summary>
			/// <param name="p"></param>
			public PointerT(PointerT p)
			{
				ptr = p.ptr;
				count = p.count;
			}

			/// <summary>
			/// constructor that allows caller to specify ptr and count
			/// </summary>
			/// <param name="node"></param>
			/// <param name="c"></param>
			public PointerT(NoteT node, long c)
			{
				ptr = node;
				count = c;
			}
		}
		private PointerT Head;
		private PointerT Tail;

		public NonBlockingQueue()
		{
			NoteT node = new NoteT();
			Head.ptr = Tail.ptr = node;
		}

		/// <summary>
		/// CAS
		/// stands for Compare And Swap
		/// Interlocked Compare and Exchange operation
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="compared"></param>
		/// <param name="exchange"></param>
		/// <returns></returns>
		private bool CAS(ref PointerT destination, PointerT compared, PointerT exchange)
		{
			if (compared.ptr == Interlocked.CompareExchange(ref destination.ptr, exchange.ptr, compared.ptr))
			{
				Interlocked.Exchange(ref destination.count, exchange.count);
				return true;
			}

			return false;
		}

		public bool Dequeue(ref T t)
		{
			PointerT head;

			// Keep trying until deque is done
			bool bDequeNotDone = true;
			while (bDequeNotDone)
			{
				// read head
				head = Head;

				// read tail
				PointerT tail = Tail;

				// read next
				PointerT next = head.ptr.next;

				// Are head, tail, and next consistent?
				if (head.count == Head.count && head.ptr == Head.ptr)
				{
					// is tail falling behind
					if (head.ptr == tail.ptr)
					{
						// is the queue empty?
						if (null == next.ptr)
						{
							// queue is empty cannnot dequeue
							return false;
						}

						// Tail is falling behind. try to advance it
						CAS(ref Tail, tail, new PointerT(next.ptr, tail.count + 1));

					} // endif

					else // No need to deal with tail
					{
						// read value before CAS otherwise another deque might try to free the next node
						t = next.ptr.value;

						// try to swing the head to the next node
						if (CAS(ref Head, head, new PointerT(next.ptr, head.count + 1)))
						{
							bDequeNotDone = false;
						}
					}

				} // endif

			} // endloop

			// dispose of head.ptr
			return true;
		}

		public void Enqueue(T t)
		{
			// Allocate a new node from the free list
			NoteT node = new NoteT();

			// copy enqueued value into node
			node.value = t;

			// keep trying until Enqueue is done
			bool bEnqueueNotDone = true;

			while (bEnqueueNotDone)
			{
				// read Tail.ptr and Tail.count together
				PointerT tail = Tail;

				// read next ptr and next count together
				PointerT next = tail.ptr.next;

				// are tail and next consistent
				if (tail.count == Tail.count && tail.ptr == Tail.ptr)
				{
					// was tail pointing to the last node?
					if (null == next.ptr)
					{
						if (CAS(ref tail.ptr.next, next, new PointerT(node, next.count + 1)))
						{
							bEnqueueNotDone = false;
						} // endif

					} // endif

					else // tail was not pointing to last node
					{
						// try to swing Tail to the next node
						CAS(ref Tail, tail, new PointerT(next.ptr, tail.count + 1));
					}

				} // endif

			} // endloop
		}

	}
}
