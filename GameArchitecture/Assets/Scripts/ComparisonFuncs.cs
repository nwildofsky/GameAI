using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool Compare(object a, object b);

static class Comparisons
{
    public static bool BoolEqual(object a, object b)
    {
        if (a == null || b == null || a.GetType() != typeof(bool) || b.GetType() != typeof(bool))
            return false;

        return a.Equals(b);
    }

    public static bool IntLesserEqual(object a, object b)
    {
        if (a == null || b == null || a.GetType() != typeof(int) || b.GetType() != typeof(int))
            return false;

        return (int)a <= (int)b;
    }

    public static bool IntGreaterEqual(object a, object b)
    {
        if (a == null || b == null || a.GetType() != typeof(int) || b.GetType() != typeof(int))
            return false;

        return (int)a >= (int)b;
    }
}
