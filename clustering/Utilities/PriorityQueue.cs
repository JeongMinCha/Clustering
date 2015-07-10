using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace clustering
{
	/// <summary>
	/// Priority queue implemented by the binary heap using list.
	/// </summary>
	public class PriorityQueue<T> 
		where T : IComparable<T>, IPriorityEntity<T>, IIdenticable
	{
		#region Private Member Variables
		private readonly List<T> data;
		#endregion

		public T this [int index]
		{
			get {
				return data[index];
			}
		}

		public PriorityQueue()
		{
			data = new List<T>();
		}
			
		/// <summary> 
		/// Returns index of the specified item in list used by binary heap. 
		/// </summary>
		public int HeapIndex (T item)
		{
			for (int index=0; index<data.Count; ++index)
			{
				if (data[index].ID == item.ID)
					return index;
			}
			return -1;
		}

		/// <summary> Updates the priority of the specified item. </summary>
		public void UpdatePriority (T item, float newPriority)
		{
			int idx = HeapIndex(item);
			if (idx == -1)
				throw new ArgumentException("The item was not found.");

			float oldPriority = data[idx].Priority;
			float difference = newPriority - oldPriority;
			data[idx].Priority = newPriority;

			if (difference < 0)			// When priority decreased.
			{
				BubbleUp(idx);
			}
			else if (difference > 0)	// When priroity increased.
			{
				BubbleDown(idx);
			}
		}

		/// <summary> Decrease the priority of the specified item. </summary>
		public void Decrease (T item, float diff)
		{
			UpdatePriority(item, item.Priority-diff);
		}

		public void Swap (int idx1, int idx2)
		{
			T tmp = data[idx1];
			data[idx1] = data[idx2];
			data[idx2] = tmp;
		}

		/// <summary>
		/// Bubbles up.
		/// </summary>
		public void BubbleUp (int index)
		{
			while (index > 0)
			{
				int parent = (index-1) >> 1;

				if (data[index].CompareTo(data[parent]) >= 0)
				{
					return;
				}
				Swap (index, parent);
				index = parent;
			}
		}

		/// <summary>
		/// Bubbles Down. This is also called heapifiy.
		/// </summary>
		public void BubbleDown (int index)
		{
			int lastParent = (data.Count-2) >> 1;
			while (index <= lastParent)
			{
				int leftChild = (index << 1) + 1;
				int rightChild = leftChild + 1;
				int min = index;

				if (leftChild < data.Count &&
					data[index].CompareTo(data[leftChild]) >= 0)
				{
					min = leftChild;
				}
				if (rightChild < data.Count &&
					data[index].CompareTo(data[rightChild]) >= 0)
				{
					min = rightChild;
				}

				// stop condition: index element is not less than both children.
				if (min == index)
				{
					return;
				}
				Swap(index, min);
				index = min;
			}
		}

		/// <summary> 
		/// Insert the specified item with the specified priority
		/// It takes time O(log n)
		/// </summary>
		public void Insert (T item, float priority)
		{
			item.Priority = priority;
			data.Add(item);
			int childIdx = data.Count-1;
			BubbleUp(childIdx);
		}

		/// <summary> 
		/// Extract the item with lowest priority value. 
		/// It takes time O(log n)
		/// </summary>
		public T Dequeue()
		{
			if (data.Count == 0) {
				throw new InvalidOperationException
					("There's no element in the priority queue.");
			}

			int lastIdx = data.Count-1;
			var firstItem = data[0];
			Swap(0, lastIdx);
			data.RemoveAt(lastIdx);
			BubbleDown(0);	// bubble down from root

			return firstItem;
		}

		public T Peek()
		{
			T firstItem = data[0];
			return firstItem;
		}

		public int Count()
		{
			return data.Count;
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			foreach (T t in data)
			{
				sb.Append(t.ToString()).Append(" ");
			}
			sb.Append("count = ").Append(data.Count.ToString());
			return sb.ToString();
		}

		/// <summary>
		/// Determines whether this instance is consistent.
		/// </summary>
		public bool IsConsistent()
		{
			// is the heap property true for all data?
			if (data.Count == 0)
				return true;

			int lastIdx = data.Count - 1;
			for (int parentIdx = 0; parentIdx < data.Count; ++parentIdx)
			{
				int lChildIdx = parentIdx * 2 + 1;
				int rChildIdx = parentIdx * 2 + 2;

				// If left child exists and 
				// parent is larger than left child, not consistent.
				if (lChildIdx <= lastIdx &&
					data[parentIdx].CompareTo(data[lChildIdx]) > 0)
				{
					return false;
				}
				// If right child exists and
				// parent is larger than right child, not constistent.
				if (rChildIdx <= lastIdx &&
					data[parentIdx].CompareTo(data[rChildIdx]) > 0)
				{
					return false;
				}
			}
			// passed all checks
			return true;
		}	// IsConsistent()
	}	// PriorityQueue
}