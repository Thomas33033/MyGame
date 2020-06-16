using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class MathfHelper
{

    public static float PixelDistance(Vector3 pointSrc, Vector3 pointDist)
    {
        //double pow = Math.Pow(pointDist.x - pointSrc.x, 2) + Math.Pow(pointDist.z - pointSrc.z, 2);
        //return (int)Math.Sqrt(pow);
        return Vector3.Distance(pointSrc, pointDist);
    }

    public static int[] StringToInt(string[] strs)
    {
        int[] array = new int[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            array[i] = int.Parse(strs[i]);
        }
        return array;
    }
}



