using UnityEngine;
using UnityEditor;

public class Tools 
{
    public static Vector3 ToVector3(string posStr)
    {
        string[] pos = posStr.Split('-');
        Vector3 v = new Vector3();
        v.x = float.Parse(pos[0]);
        v.y = float.Parse(pos[1]);
        v.z = float.Parse(pos[2]);
        return v;
    }

    public static string Vector3ToString(Vector3 v)
    {
        return v.x + "," + v.y + "," + v.z;
    }

}