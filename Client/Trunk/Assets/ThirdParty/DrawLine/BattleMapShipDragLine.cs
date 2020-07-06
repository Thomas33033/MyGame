using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BattleMapShipDragLine : MonoBehaviour
{

    private MeshFilter mFilter;
    public int uvScale = 10;
    public float width = 1;
    public float minDis = 0.1f, maxDis = 0.2f;

    //[HideInInspector]
    public List<Vector3> points;
    private Mesh mesh;
    BattleMapFaceData fd;
    private int faceNum;

    public float rade, uvRade;

    public List<int> listNode;

    void Awake()
    {
        mesh = new Mesh();
        mFilter = GetComponent<MeshFilter>();
        // mCollider=GetComponent<MeshCollider>();

        points = new List<Vector3>();
        listNode = new List<int>();

        uvRade = 1f / uvScale;
        width = width * 0.5f;
        rade = 1f / minDis;
    }
   
    public void Clear()
    {
        mesh.Clear();
        points.Clear();
        listNode.Clear();
        fd = null;
        if (mFilter != null) mFilter.mesh = null;
    }

	public void AddPoint(Vector3 pos)
	{
		if (points.Count > 0)
		{
			float dis = Vector3.Distance(points[points.Count - 1], pos);
			if (points.Count > 1)
			{
				float tempDis = Vector3.Distance(points[points.Count - 2], pos);
				if (tempDis < minDis)
				{
					points[points.Count - 1] = pos;
                    LinkPoint();
					Draw();
					return;
				}
			}

			int num = (int)(dis * rade);

			Vector3 nor = (pos - points[points.Count - 1]).normalized;
			Vector3 end = points[points.Count - 1];

			for (int i = 1; i <= num; i++)
			{
				points.Add(end + nor * i * minDis);
                //testObj.transform.position=end+nor*i*minDis;
                LinkPoint();
				faceNum++;
			}

		}
		else
		{
			points.Add(pos);

		}
		Draw();
	}
    

    public void DelStart()
    {
        if (fd != null && fd.p.Length > 4)
        {
            fd = BattleMapFaceData.DelStart(fd);
            mFilter.mesh = WallMesh(fd);
            //mCollider.sharedMesh=mFilter.mesh;
        }
        if (points.Count > 0)
        {
            points.RemoveAt(0);
        }

    }

    void LinkPoint()
    {
        if (points.Count > 2)
        {
            Vector3 nor = (points[points.Count - 1] - points[points.Count - 2]);

            nor = GetPerpendicular(nor).normalized;
   
            Vector3 p1 = points[points.Count - 2] - nor * width;
            Vector3 p2 = points[points.Count - 2] + nor * width;
            Vector3 p3 = points[points.Count - 1] + nor * width;
            Vector3 p4 = points[points.Count - 1] - nor * width;

            fd.p[fd.p.Length - 1] = p1;
            fd.p[fd.p.Length - 2] = p2;
            //lineData[lineData.Count-1].p4=p1;
            //lineData[lineData.Count-1].p3=p2;
            BattleMapInputData tt = new BattleMapInputData(p1, p2, p3, p4);
            //lineData.Add(tt);
            BattleMapFaceData temp = PointToFace(tt);
            temp.uv[0].x = temp.uv[1].x = (faceNum % uvScale) * uvRade;
            temp.uv[2].x = temp.uv[3].x = (faceNum % uvScale + 1) * uvRade;
            fd = BattleMapFaceData.Join(fd, temp);

        }
        else if (points.Count > 1)
        {
            Vector3 nor = (points[points.Count - 1] - points[points.Count - 2]).normalized;
            Vector3 p1 = points[points.Count - 2] - GetPerpendicular(nor) * width;
            Vector3 p2 = points[points.Count - 2] + GetPerpendicular(nor) * width;
            Vector3 p3 = points[points.Count - 1] + GetPerpendicular(nor) * width;
            Vector3 p4 = points[points.Count - 1] - GetPerpendicular(nor) * width;
            BattleMapInputData tt = new BattleMapInputData(p1, p2, p3, p4);
            // lineData.Add(tt);
            fd = PointToFace(tt);
            fd.uv[0].x = fd.uv[1].x = (faceNum % uvScale) * uvRade;
            fd.uv[2].x = fd.uv[3].x = (faceNum % uvScale + 1) * uvRade;
        }  
    }
 

    public void Draw()
    {

        if (fd != null)
        {
            mFilter.mesh = WallMesh(fd);

            // mCollider.sharedMesh=mFilter.mesh;
        }

    }
    public Mesh WallMesh(BattleMapFaceData face)
    {
        mesh.Clear();
        mesh.vertices = face.p;
        mesh.uv = face.uv;
        mesh.triangles = face.triangle;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        //mesh.Optimize();
        return mesh;
    }
    //返回线段在(x,y) 平面上的垂线
    public Vector3 GetPerpendicular(Vector3 dir)
    {
        return new Vector3(-dir.y, dir.x, dir.z);
    }

    private float IntersectionDis(Vector3 nor, Vector3 dir, float width)
    {
        float angle = Vector3.Angle(nor, dir);
        return width / (Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public BattleMapFaceData PointToFace(BattleMapInputData points)
    {
        BattleMapFaceData w1 = ToFace(points);

        return w1;
    }

    //获得每个面的顶点信息
    private BattleMapFaceData ToFace(BattleMapInputData points)
    {
        Vector3[] rt = new Vector3[4];
        rt = new Vector3[] { points.p1, points.p2, points.p3, points.p4 };

        return new BattleMapFaceData(new BattleMapInputData(rt, (points.p2 - points.p1).magnitude));
    }

    public void AddNode(Vector3 p)
    {
        int n = points.Count;
        AddPoint(p);
        listNode.Add(points.Count - n);
    }

    public void DelNode()
    {
        if (listNode.Count == 0)
        {
            Clear();
        }
        else
        {
            for (int i = 0; i < listNode[0]; i++)
            {
                DelStart();
            }
            listNode.RemoveAt(0);
        }
    }
}
