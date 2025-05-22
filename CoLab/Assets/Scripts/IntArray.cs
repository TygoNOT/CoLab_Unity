using Unity.Netcode;
using System;

[Serializable]
public struct IntArray : IEquatable<IntArray>
{
    public int[] Array;

    public bool Equals(IntArray other)
    {
        if (Array == null || other.Array == null)
            return Array == other.Array;

        if (Array.Length != other.Array.Length)
            return false;

        for (int i = 0; i < Array.Length; i++)
        {
            if (Array[i] != other.Array[i])
                return false;
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return obj is IntArray other && Equals(other);
    }

    public override int GetHashCode()
    {
        if (Array == null) return 0;

        unchecked
        {
            int hash = 17;
            foreach (int item in Array)
            {
                hash = hash * 23 + item.GetHashCode();
            }
            return hash;
        }
    }
}