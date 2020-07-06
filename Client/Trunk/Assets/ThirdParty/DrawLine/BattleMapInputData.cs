using UnityEngine;

public class BattleMapInputData
{
    public Vector3 p1 { get { return points[0]; } set { points[0] = value; } }
    public Vector3 p2 { get { return points[1]; } set { points[1] = value; } }
    public Vector3 p3 { get { return points[2]; } set { points[2] = value; } }
    public Vector3 p4 { get { return points[3]; } set { points[3] = value; } }
    public Vector3[] points;
    public float height;

    public BattleMapInputData()
    {
        points = new Vector3[4];
    }
    public BattleMapInputData(Vector3[] points, float height = 0)
    {
        this.points = points;
        this.height = height;
    }
    public BattleMapInputData(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        points = new Vector3[4];
        points[0] = p1;
        points[1] = p2;
        points[2] = p3;
        points[3] = p4;
    }
}
