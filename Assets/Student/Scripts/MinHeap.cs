using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// heap class that can handle DijPrioHolders.
/// also stores the positions of each item in a hash table to 
/// allow quick access to their positions.
/// </summary>
public class MinHeap
{
    Dictionary<Vector2Int, int> valueToKey;

    DijPrioHolder[] pq;
    int n;


    // create minheap with starting capacity
    public MinHeap(int capacity)
    {
        valueToKey = new Dictionary<Vector2Int, int>();
        
        pq = new DijPrioHolder[capacity + 1];
        n = 0;


    }

    // check if heap is enmpty
    public bool IsEmpty()
    {
        return n <= 0;
    }


    // get value at first spot
    public DijPrioHolder Min()
    {
        if (IsEmpty()) { throw new System.Exception(); }
        return pq[1];
    }

    // resize heap array
    void Resize(int cap)
    {
        DijPrioHolder[] aux = new DijPrioHolder[cap];

        for (int i = 1; i <= n; i++)
        {
            aux[i] = pq[i];
        }
        pq = aux;

    }

    // insert new value to heap
    public void Insert(DijPrioHolder item)
    {
        if (n == pq.Length - 1) { Resize(pq.Length * 2); }
        n++;
        pq[n] = item;
        valueToKey.Add(item.vertex, n);
        Swim(n);
    }

    // take and delete the first value in heap
    public DijPrioHolder DelMin()
    {
        if (IsEmpty()) { return null; }
        DijPrioHolder min = pq[1];
        Exch(1, n);
        pq[n] = null;
        n--;
        Sink(1);
        

        if (!(min is null)) { valueToKey.Remove(min.vertex); }
        

        return min;
    }

    // move item upwards in heap
    void Swim(int k)
    {
        while(k > 1 && Greater(k/2, k))
        {
            Exch(k / 2, k);
            k = k / 2;
        }
    }

    // move item downwards in heap
    void Sink(int k) 
    {
        while(2*k <= n)
        {
            int j = k * 2;
            if (j < n && Greater(j, j + 1))
            {
                j++;
            }
            if (!Greater(k, j)) { break; }
            Exch(k, j);
            k = j;
        }
    }

    // is value greater
    bool Greater(int i, int j)
    {
        if (pq[j] is null)
        {
            return true;
        }
        if (pq[i] is null)
        {
            return false;
        }
        return pq[j] < pq[i];
    }

    // swap places with two values
    void Exch(int i, int j)
    {
        DijPrioHolder aux = new DijPrioHolder() {vertex = pq[i].vertex, cost = pq[i].cost };
        pq[i] = new DijPrioHolder() {vertex = pq[j].vertex, cost = pq[j].cost };
        pq[j] = new DijPrioHolder() {vertex = aux.vertex, cost = aux.cost};

        valueToKey[pq[i].vertex] = i; 
        valueToKey[pq[j].vertex] = j;
    }

    // change the priority of an item
    // only swims because we know we will only change value if it is smaller
    public void ChangePriority(Vector2Int vertex, float newPrio)
    {
        int pos = valueToKey[vertex];
        pq[pos].cost = newPrio;
        Swim(pos);
    }

    // check if heap contains a value
    public bool Contains(Vector2Int vertex)
    {
        return valueToKey.ContainsKey(vertex);
    }

    // testing, write out all values in heap, in order.
    public void PrintStructure()
    {
        for(int i = 1; i <= n; i++)
        {
            Debug.Log("item " + i + ": " + pq[i].vertex + " " + pq[i].cost);
        }
    }
}
