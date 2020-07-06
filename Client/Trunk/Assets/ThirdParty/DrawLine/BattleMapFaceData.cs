using UnityEngine;
using System.Collections.Generic;

public class BattleMapFaceData
{
    public Vector3[] p;
    public Vector2[] uv;
    public int[] triangle;
    public static Vector2 uvPos;
    public static Vector2 point = new Vector2(1, 1);
    public static int pointNum = 0;

    public BattleMapFaceData()
    {
        p = new Vector3[4];
        uv = new Vector2[4];
        triangle = new int[] { 0, 1, 3, 0, 2, 3 };
    }

    //计算mesh信息
    public BattleMapFaceData(BattleMapInputData d, int i = 0, float dis = 1)
    {
        triangle = new int[] { 0, 1, 2, 0, 2, 3 };
        p = d.points;
        uv = new Vector2[4];

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

    }


    //二维旋转矩阵
    private Vector2 Matrix(Vector2 p, Vector2 angle)
    {
        float c = Mathf.Sqrt(angle.x * angle.x + angle.y * angle.y);
        //Vector2 v2 = new Vector2(1, 1);
        Vector4 v4 = new Vector4(angle.x / c, angle.y / c, -angle.y / c, angle.x / c);
        return new Vector2(p.x * v4.x + p.y * v4.y, p.x * v4.z + p.y * v4.w);
    }
    private float Rad(float x, float z)
    {
        float r;
        float k = x / z;
        if (k > 0.00001f) r = Mathf.Atan(k);
        else if (k < 0.00001 && k > -0.00001)
        {
            if (z < 0)
                r = Mathf.PI;
            else
                r = 0;
        }
        else
            r = Mathf.PI + Mathf.Atan(k);
        return r;
    }
    //合并mesh信息
    public static BattleMapFaceData Join(BattleMapFaceData a, BattleMapFaceData b)
    {

        BattleMapFaceData rt = new BattleMapFaceData();
        List<Vector3> v = new List<Vector3>();
        v.AddRange(a.p);
        v.AddRange(b.p);
        List<Vector2> uv = new List<Vector2>();
        List<int> tri = new List<int>();
        tri.AddRange(a.triangle);
        tri.AddRange(b.triangle);

        int[] k = new int[a.triangle.Length + b.triangle.Length];
        for (int i = 0; i < a.triangle.Length + b.triangle.Length; i++)
        {
            k[i] = ((int)(i / 6)) * 4 + b.triangle[i % 6];
        }
        rt.p = v.ToArray();
        uv.AddRange(a.uv);
        uv.AddRange(b.uv);
        rt.uv = uv.ToArray();
        rt.triangle = k;
        return rt;
    }


    public static BattleMapFaceData DelStart(BattleMapFaceData a)
    {

        BattleMapFaceData rt = new BattleMapFaceData();
        List<Vector3> v = new List<Vector3>();
        v.AddRange(a.p);
        for (int i = 0; i < 4; i++)
        {
            v.RemoveAt(0);
        }

        List<Vector2> uv = new List<Vector2>();
        uv.AddRange(a.uv);
        for (int i = 0; i < 4; i++)
        {
            uv.RemoveAt(0);
        }

        List<int> tri = new List<int>();
        tri.AddRange(a.triangle);
        for (int i = 0; i < 6; i++)
        {
            tri.RemoveAt(0);
        }
        int[] triangle = new int[] { 0, 1, 2, 0, 2, 3 };
        for (int i = 0; i < tri.Count; i++)
        {
            tri[i] = ((int)(i / 6)) * 4 + triangle[i % 6];
        }
        rt.p = v.ToArray();
        rt.uv = uv.ToArray();
        rt.triangle = tri.ToArray();
        return rt;
    }

}
