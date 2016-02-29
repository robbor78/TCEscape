/******************************************************************************
 *  Compilation:  javac MinPQ.java
 *  Execution:    java MinPQ < input.txt
 *  Dependencies: StdIn.java StdOut.java
 *  
 *  Generic min priority queue implementation with a binary heap.
 *  Can be used with a comparator instead of the natural order.
 *
 *  % java MinPQ < tinyPQ.txt
 *  E A E (6 left on pq)
 *
 *  We use a one-based array to simplify parent and child calculations.
 *
 *  Can be optimized by replacing full exchanges with half exchanges
 *  (ala insertion sort).
 *
 ******************************************************************************/



using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
/**
*  The <tt>MinPQ</tt> class represents a priority queue of generic keys.
*  It supports the usual <em>insert</em> and <em>delete-the-minimum</em>
*  operations, along with methods for peeking at the minimum key,
*  testing if the priority queue is empty, and iterating through
*  the keys.
*  <p>
*  This implementation uses a binary heap.
*  The <em>insert</em> and <em>delete-the-minimum</em> operations take
*  logarithmic amortized time.
*  The <em>min</em>, <em>size</em>, and <em>is-empty</em> operations take constant time.
*  Construction takes time proportional to the specified capacity or the number of
*  items used to initialize the data structure.
*  <p>
*  For additional documentation, see <a href="http://algs4.cs.princeton.edu/24pq">Section 2.4</a> of
*  <i>Algorithms, 4th Edition</i> by Robert Sedgewick and Kevin Wayne.
*
*  @author Robert Sedgewick
*  @author Kevin Wayne
*
*  @param <Key> the generic type of key on this priority queue
*/
public class MinPQ<Key> : IEnumerable<Key>
{
  private Key[] pq;                    // store items at indices 1 to N
  private int N;                       // number of items on priority queue
  private Comparer<Key> comparator;  // optional comparator

  /**
   * Initializes an empty priority queue with the given initial capacity.
   *
   * @param  initCapacity the initial capacity of this priority queue
   */
  public MinPQ(int initCapacity)
  {
    init(initCapacity);
  }

  /**
   * Initializes an empty priority queue.
   */
  public MinPQ() : this(1)
  {
  }

  /**
   * Initializes an empty priority queue with the given initial capacity,
   * using the given comparator.
   *
   * @param  initCapacity the initial capacity of this priority queue
   * @param  comparator the order to use when comparing keys
   */
  public MinPQ(int initCapacity, Comparer<Key> comparator)
  {
    this.comparator = comparator;
    init(initCapacity);
  }

  /**
   * Initializes an empty priority queue using the given comparator.
   *
   * @param  comparator the order to use when comparing keys
   */
  public MinPQ(Comparer<Key> comparator) : this(1, comparator)
  {
  }

  /**
   * Initializes a priority queue from the array of keys.
   * <p>
   * Takes time proportional to the number of keys, using sink-based heap construction.
   *
   * @param  keys the array of keys
   */
  public MinPQ(Key[] keys)
  {
    N = keys.Length;
    pq = new Key[keys.Length + 1];
    for (int i = 0; i < N; i++)
      pq[i + 1] = keys[i];
    for (int k = N / 2; k >= 1; k--)
      sink(k);
    Debug.Assert(isMinHeap());
  }

  /**
   * Returns true if this priority queue is empty.
   *
   * @return <tt>true</tt> if this priority queue is empty;
   *         <tt>false</tt> otherwise
   */
  public bool isEmpty()
  {
    return N == 0;
  }

  /**
   * Returns the number of keys on this priority queue.
   *
   * @return the number of keys on this priority queue
   */
  public int size()
  {
    return N;
  }

  /**
   * Returns a smallest key on this priority queue.
   *
   * @return a smallest key on this priority queue
   * @throws NoSuchElementException if this priority queue is empty
   */
  public Key min()
  {
    if (isEmpty()) throw new Exception("Priority queue underflow");
    return pq[1];
  }

  // helper function to double the size of the heap array
  private void resize(int capacity)
  {
    Debug.Assert(capacity > N);
    Key[] temp = new Key[capacity];
    for (int i = 1; i <= N; i++)
    {
      temp[i] = pq[i];
    }
    pq = temp;
  }

  /**
   * Adds a new key to this priority queue.
   *
   * @param  x the key to add to this priority queue
   */
  public void insert(Key x)
  {
    // double size of array if necessary
    if (N == pq.Length - 1) resize(2 * pq.Length);

    // add x, and percolate it up to maintain heap invariant
    pq[++N] = x;
    swim(N);
    Debug.Assert(isMinHeap());
  }

  /**
   * Removes and returns a smallest key on this priority queue.
   *
   * @return a smallest key on this priority queue
   * @throws NoSuchElementException if this priority queue is empty
   */
  public Key delMin()
  {
    if (isEmpty()) throw new Exception("Priority queue underflow");
    exch(1, N);
    Key min = pq[N--];
    sink(1);
    pq[N + 1] = default(Key);         // avoid loitering and help with garbage collection
    if ((N > 0) && (N == (pq.Length - 1) / 4)) resize(pq.Length / 2);
    Debug.Assert(isMinHeap());
    return min;
  }


  /***************************************************************************
   * Helper functions to restore the heap invariant.
   ***************************************************************************/

  private void swim(int k)
  {
    while (k > 1 && greater(k / 2, k))
    {
      exch(k, k / 2);
      k = k / 2;
    }
  }

  private void sink(int k)
  {
    while (2 * k <= N)
    {
      int j = 2 * k;
      if (j < N && greater(j, j + 1)) j++;
      if (!greater(k, j)) break;
      exch(k, j);
      k = j;
    }
  }

  /***************************************************************************
   * Helper functions for compares and swaps.
   ***************************************************************************/
  private bool greater(int i, int j)
  {
    if (comparator == null)
    {
      return ((IComparable<Key>)pq[i]).CompareTo(pq[j]) > 0;
    }
    else {
      return comparator.Compare(pq[i], pq[j]) > 0;
    }
  }

  private void exch(int i, int j)
  {
    Key swap = pq[i];
    pq[i] = pq[j];
    pq[j] = swap;
  }

  // is pq[1..N] a min heap?
  private bool isMinHeap()
  {
    return isMinHeap(1);
  }

  // is subtree of pq[1..N] rooted at k a min heap?
  private bool isMinHeap(int k)
  {
    if (k > N) return true;
    int left = 2 * k, right = 2 * k + 1;
    if (left <= N && greater(k, left)) return false;
    if (right <= N && greater(k, right)) return false;
    return isMinHeap(left) && isMinHeap(right);
  }


  private void init(int initCapacity)
  {
    pq = new Key[initCapacity + 1];
    N = 0;
  }

  /**
   * Returns an iterator that iterates over the keys on this priority queue
   * in ascending order.
   * <p>
   * The iterator doesn't implement <tt>remove()</tt> since it's optional.
   *
   * @return an iterator that iterates over the keys in ascending order
   */

  public IEnumerator<Key> GetEnumerator()
  {
    return new HeapIterator(this);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  private class HeapIterator : IEnumerator<Key>
  {
    // create a new pq
    private MinPQ<Key> copy;

    public Key Current
    {
      get
      {
        return copy.delMin();
      }
    }

    //object IEnumerator.Current
    //{
    //  get
    //  {
    //    throw new NotImplementedException();
    //  }
    //}

    // add all items to copy of heap
    // takes linear time since already in heap order so no keys move
    public HeapIterator(MinPQ<Key> queue)
    {
      if (queue.comparator == null) copy = new MinPQ<Key>(queue.size());
      else copy = new MinPQ<Key>(queue.size(), queue.comparator);
      for (int i = 1; i <= queue.N; i++)
        copy.insert(queue.pq[i]);
    }

    public bool hasNext() { return !copy.isEmpty(); }


    public void Dispose()
    {
      
    }

    public bool MoveNext()
    {
      if (!hasNext()) throw new NoSuchElementException();
      return copy.delMin();
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }
  }

}

/******************************************************************************
 *  Copyright 2002-2015, Robert Sedgewick and Kevin Wayne.
 *
 *  This file is part of algs4.jar, which accompanies the textbook
 *
 *      Algorithms, 4th edition by Robert Sedgewick and Kevin Wayne,
 *      Addison-Wesley Professional, 2011, ISBN 0-321-57351-X.
 *      http://algs4.cs.princeton.edu
 *
 *
 *  algs4.jar is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  algs4.jar is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with algs4.jar.  If not, see http://www.gnu.org/licenses.
 ******************************************************************************/
