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


    public static bool CompareVersion(string newVersion, string oldVersion)
    {
        string[] newArr = newVersion.Split(',');
        string[] oldArr = oldVersion.Split(',');

        for (int i = 0; i < 4; i++)
        {
            string newCode = newArr[i];
            string oldCode = oldArr[i];

            if (newArr[i] == null) newCode = "0";
            if (oldArr[i] == null) oldCode = "0";

            if (int.Parse(newCode) > int.Parse(oldCode))
            {
                return true;
            }
        }

        return false;
    }

}