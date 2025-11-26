using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class UFunc
{
    //returns index of vector component with largest magnitude
    public static int VectorSignificant(Vector4 v)
    {
        int max = 0;
        if (Mathf.Abs(v[max]) < Mathf.Abs(v[1])) max = 1;
        if (Mathf.Abs(v[max]) < Mathf.Abs(v[2])) max = 2;
        if (Mathf.Abs(v[max]) < Mathf.Abs(v[3])) max = 3;
        return max;
    }

    public static Vector3 TriNormal(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return Vector3.Cross(v2 - v1, v3 - v1).normalized;
    }

    public static float RoundDigit(float num, int digit)
    {
        float degree = Mathf.Pow(10, digit);
        return Mathf.Round(num * degree) / degree;
    }

    public static bool SameSign(float n1, float n2)
    {
        return Mathf.Sign(n1) == Mathf.Sign(n2);
    }
    public static bool SameSign(Vector3 n1, Vector3 n2)
    {
        return SameSign(n1.x, n2.x) && SameSign(n1.y, n2.y) && SameSign(n1.z, n2.z);
    }

    public static bool LessThanAll(float n, params float[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (n > list[i]) return false;
        }
        return true;
    }
    public static bool GreaterThanAll(float n, params float[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (n < list[i]) return false;
        }
        return true;
    }
    public static bool FurtherThanAll(float n, params float[] list) //further from zero
    {
        n = Mathf.Abs(n);
        for (int i = 0; i < list.Length; i++)
        {
            if (n < Mathf.Abs(list[i])) return false;
        }
        return true;
    }

    public static float FurthestOfList(params float[] list) //returns the element thats furtherst from zero
    {
        float furthest = list[0];
        for (int i = 1; i < list.Length; i++)
        {
            if (Mathf.Abs(list[i]) > Mathf.Abs(furthest)) furthest = list[i];
        }
        return furthest;
    }
    public static int FurthestOfListIndex(params float[] list) //returns the element thats furtherst from zero
    {
        float furthest = Mathf.Abs(list[0]);
        int index = 0;
        for (int i = 1; i < list.Length; i++)
        {
            if (Mathf.Abs(list[i]) > Mathf.Abs(furthest))
            {
                furthest = Mathf.Abs(list[i]);
                index = i;
            }
        }
        return index;
    }
    public static int ClosestOfListIndex(params float[] list) //returns the element thats closest to zero
    {
        float furthest = Mathf.Abs(list[0]);
        int index = 0;
        for (int i = 1; i < list.Length; i++)
        {
            if (Mathf.Abs(list[i]) < Mathf.Abs(furthest))
            {
                furthest = Mathf.Abs(list[i]);
                index = i;
            }
        }
        return index;
    }

    public static int MinIndex(params float[] list)
    {
        int index = 0;
        for (int i = 1; i < list.Length; i++)
        {
            if (list[i] < list[index])
            {
                index = i;
            }
        }
        return index;
    }
    public static int MaxIndex(params float[] list)
    {
        int index = 0;
        for (int i = 1; i < list.Length; i++)
        {
            if (list[i] > list[index])
            {
                index = i;
            }
        }
        return index;
    }

    public static bool IsGreaterInDirection(float n1, float n2, float dir) //is number greater in specified direction
    {
        if (dir >= 0)
        {
            return n1 > n2;
        }
        return n1 < n2;
    }

    public static float Clamp01(float n)
    {
        return Mathf.Clamp(n, 0, 1);
    }
    public static float Clamp1(float n)
    {
        return Mathf.Clamp(n, -1, 1);
    }

    public static bool SameQuadrant(Vector2 v1, Vector2 v2)
    {
        return SameSign(v1.x, v2.x) && SameSign(v1.y, v2.y);
    }

    public static Vector3 VectorBasisShift(Vector3 vec, Vector3 xBase, Vector3 yBase, Vector3 zBase)
    {
        return new Vector3(
            vec.x * xBase.x + vec.y * yBase.x + vec.z * zBase.x,
            vec.x * xBase.y + vec.y * yBase.y + vec.z * zBase.y,
            vec.x * xBase.z + vec.y * yBase.z + vec.z * zBase.z
        );
    }

    public static float SqrSum(params float[] par)
    {
        float sum = 0;
        for (int i = 0; i < par.Length; i++)
        {
            sum += par[i] * par[i];
        }
        return sum;
    }

    public static float RepeatRange(float n, float lo, float hi)
    {
        if (hi < lo)
        {
            (lo, hi) = (hi, lo);
        }
        return Mathf.Repeat(n - lo, hi - lo) + lo;
    }

    public static bool CloseTo(float n, float target, float range)
    {
        return n <= target + range && n >= target - range;
    }

    public static void PrintList(params string[] par)
    {
        string s = "";
        for (int i = 0; i < par.Length; i++)
        {
            s = s + par[i] + " ";
        }
        Debug.Log(s);
    }
    public static void PrintList(params float[] par)
    {
        string s = "";
        for (int i = 0; i < par.Length; i++)
        {
            s = s + par[i].ToString() + " ";
        }
        Debug.Log(s);
    }

    public static T[] List2Array<T>(List<T> list)
    {
        T[] ar = new T[list.Count];
        for (int i = 0; i < ar.Length; i++)
        {
            ar[i] = list[i];
        }
        return ar;
    }

    public static float TickTimer(ref float time)
    {
        time = Mathf.Max(time - Time.deltaTime, 0);
        return time;
    }
    public static float TickTimerFixed(ref float time)
    {
        time = Mathf.Max(time - Time.fixedDeltaTime, 0);
        return time;
    }
}
