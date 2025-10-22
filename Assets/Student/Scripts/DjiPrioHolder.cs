using System;
using UnityEngine;

public class DijPrioHolder : IComparable<DijPrioHolder>
{
    public Vector2Int vertex;
    public float cost;




    public static bool operator <(DijPrioHolder left, DijPrioHolder right)
    {
        return (left.cost < right.cost);
    }

    public static bool operator >(DijPrioHolder left, DijPrioHolder right)
    {
        return (left.cost > right.cost);
    }

    public static bool operator ==(DijPrioHolder left, DijPrioHolder right)
    {
        return (left.vertex == right.vertex);
    }

    public static bool operator !=(DijPrioHolder left, DijPrioHolder right)
    {
        return (left.vertex != right.vertex);
    }

    public int CompareTo(DijPrioHolder other)
    {
        return this.cost.CompareTo(other.cost);
    }
}
