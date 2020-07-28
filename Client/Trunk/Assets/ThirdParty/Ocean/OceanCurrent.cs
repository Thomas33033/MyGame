using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;
using UnityEngine.SocialPlatforms;
/// <summary>
/// 洋流
/// </summary>
public class OceanCurrent : MonoBehaviour
{
 
    public float speed_x = 2;
    public float speed_y = 0;
    public List<GameObject> pathObj;
    public List<Vector3> pathPoints = new List<Vector3>();
    private BattleMapShipDragLine line;
    public float offset;
    float offset_y;
    private Material material;

    public float outOceanDis = 0.5f;
    public SpriteRenderer head, end;

    public float r, g, b, a;
   // private bool freeze = false;
    public bool editor = false;
    public GameObject mode;
    private bool initialize = false;
    // Use this for initialization

    Vector3[] path;
    public void Start() {

        List<Vector3> point = new List<Vector3>();
        for (int i = 0; i < pathObj.Count; i++)
        {
            point.Add(pathObj[i].transform.position);
        }

        path = point.ToArray();

        material = mode.GetComponent<MeshRenderer>().material;
        if(!initialize)
        {
            Init();
        }
    }

    public void Update()
    {
        offset -= Time.deltaTime * speed_x;
        offset_y += Time.deltaTime * speed_y;
        material.SetTextureOffset("_MainTex", new Vector2(offset, offset_y));

        if (offset < -1)
            offset += 1;

    }

    public void Init()
    {

        initialize = true;
        line = mode.GetComponent<BattleMapShipDragLine>();
       

       

        pathPoints.AddRange(WaypointManager.GetCurvedPoints(path));

        for (int i = 0; i<pathPoints.Count; ++i)
        {
            line.AddPoint(pathPoints[i]);
        }

        head.transform.up = pathPoints[0] - pathPoints[1];
        head.transform.position=pathPoints[0]- head.transform.up*0.2f;
        
        head.color=new Color(r, g, b,a);
        end.transform.up = pathPoints[pathPoints.Count - 1] - pathPoints[pathPoints.Count - 2];
        end.transform.position=pathPoints[pathPoints.Count-1] - end.transform.up * 0.2f;
       
        end.color=new Color(r, g, b,a);


        List<Vector2> side1 = new List<Vector2>();
        List<Vector2> side2 = new List<Vector2>();
        Vector3 nor;
        Vector3 p1;
        Vector3 p2;
        for (int i = 1; i<line.points.Count; ++i)
        {
            nor = (line.points[i]-line.points[i-1]);
            nor=GetPerpendicular(nor).normalized;
            p1 = line.points[i-1]-nor*(line.width-0.05f);
            p2 = line.points[i-1]+nor*(line.width-0.05f);
            side1.Add(p1);
            side2.Add(p2);
        }

        nor = (line.points[line.points.Count-1]-line.points[line.points.Count-2]);
        nor=GetPerpendicular(nor).normalized;
        p1 = line.points[line.points.Count-1]-nor*(line.width-0.1f);
        p2 = line.points[line.points.Count-1]+nor*(line.width-0.1f);
        side1.Add(p1);
        side2.Add(p2);

        side2.Reverse();
        side1.AddRange(side2);


        PolygonCollider2D mCollider2D = gameObject.AddComponent<PolygonCollider2D>();
        mCollider2D.isTrigger=true;
        mCollider2D.SetPath(0, side1.ToArray());

        transform.position+=Vector3.forward*0.01f;
    }

    //public void Freeze(bool stage)
    //{
    //    this.stage = stage;
    //}


    //public override bool ShipEnterTrigger(PlayerShip other, Collider2D collider)
    //{
    //    EnterOceanCurrent(other);
    //    return true;
    //}


    //void EnterOceanCurrent(PlayerShip other)
    //{
    //    ship = other;
    //    float dis = 0, minDis = int.MaxValue;
    //    int index = 0;
    //    for (int i = 0; i < pathPoints.Count; ++i)
    //    {
    //        dis = Vector3.Distance(pathPoints[i], other.transform.position);
    //        if (dis <= minDis)
    //        {
    //            minDis = dis;
    //            index = i;
    //        }
    //    }

    //    if (index >= pathPoints.Count - 1)
    //    {
    //        Vector3 dir = (pathPoints[pathPoints.Count - 1] - pathPoints[pathPoints.Count - 2]).normalized;
    //        Vector3[] p = new Vector3[] { ship.transform.position, (ship.transform.position+ ship.transform.position + dir.normalized * outOceanDis)/2 , ship.transform.position + dir.normalized * outOceanDis };
    //        // OutOceanCurrent((pathPoints[pathPoints.Count - 1] - pathPoints[pathPoints.Count - 2]).normalized);
    //        other.MoveOnOceanCurrent();
    //        other.SetSpeedAcce(0.6f);
    //        other.shipMove.Move(p);
    //        return;
    //    }
    //    other.MoveOnOceanCurrent();
    //    other.SetSpeedAcce(0.4f);
    //    other.shipMove.Move(GetPath(other.transform.position));
       
    //}




    public  Vector3[] GetPath(Vector3 pos)
    {
        float dis = 99999;
        int index = 0;
        for (int i = 0; i < pathPoints.Count; i++)
        {
            if (Vector3.Distance(pathPoints[i], pos) < dis)
            {
                dis = Vector3.Distance(pathPoints[i], pos);
                index = i;
            }
        }
        Vector3[] path = new Vector3[pathPoints.Count- index+1];

        for (int i = index; i < pathPoints.Count; i++)
        {
            path[i - index] = pathPoints[i];
        }
        path[path.Length - 1] = path[path.Length - 2] + (path[path.Length - 2] - path[path.Length - 3]).normalized * outOceanDis;
        return path;
    }


    public Vector2 GetPerpendicular(Vector2 dir)
    {
        return new Vector2(-dir.y, dir.x);
    }

    void On_Drag(Gesture gesture)
    {

    }

    void On_DragStart(Gesture gesture)
    {

        //if (ship!=null&&gesture.pickedObject==ship.gameObject)
        //{
        //    if (gesture.deltaPosition.magnitude>10)
        //    {
        //        Vector3 dir = Vector3.Project(gesture.deltaPosition, ship.transform.right);
        //        dir = dir.normalized * 3f + ship.transform.up;
        //        Vector3[] p = new Vector3[] { ship.transform.position, (ship.transform.position + ship.transform.position + dir.normalized * outOceanDis) / 2, ship.transform.position + dir.normalized * outOceanDis };
        //        // OutOceanCurrent((pathPoints[pathPoints.Count - 1] - pathPoints[pathPoints.Count - 2]).normalized);
        //        ship.SetSpeedAcce(0.6f);
        //        ship.shipMove.Move(p);
        //        ship = null;
        //    }
        //}
    }

    void On_DragEnd(Gesture gesture)
    {

    }
    void OnEnable()
    {
        EasyTouch.On_DragStart += On_DragStart;
        EasyTouch.On_DragEnd += On_DragEnd;
        EasyTouch.On_Drag += On_Drag;
    }

    void OnDestroy()
    {
        EasyTouch.On_DragStart -= On_DragStart;
        EasyTouch.On_DragEnd -= On_DragEnd;
        EasyTouch.On_Drag -= On_Drag;
    }
    public void OnDisable()
    {
        EasyTouch.On_DragStart -= On_DragStart;
        EasyTouch.On_DragEnd -= On_DragEnd;
        EasyTouch.On_Drag -= On_Drag;

    }

    void GetPathsInEditor()
    {
        Transform pathNote = transform.Find("pathNote");
        if (pathNote != null)
        {
            path = new Vector3[pathNote.childCount];
            for (int i = 0; i < pathNote.childCount; i++)
            {
                path[i] = pathNote.GetChild(i).position;
            }
        }
    }
    void OnDrawGizmos()
    {
        GetPathsInEditor();
        if (path.Length < 2)
        {
            return;
        }
        //Vector3[] tempPoints = WaypointManager.GetCurvedPoints(pathPoints);
        //Debug.LogError(tempPoints.Length + "  " + pathPoints.Length);
        //for (int i = 0;i< pathPoints.Length;i++)
        //{
        //    int index = (tempPoints.Length - 1) * i / (pathPoints.Length - 1);
        //    Debug.LogError(index + " " + i + " " + tempPoints[index]);
        //}
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(path[0], Vector3.one);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(path[path.Length - 1], Vector3.one);

        Gizmos.color = Color.yellow;
        for (int i = 1; i < path.Length - 1; i++)
            Gizmos.DrawWireSphere(path[i], 0.7f);

        if (path.Length >= 2)
        {
            WaypointManager.DrawCurved(path);
        }
        else
            WaypointManager.DrawStraight(path);

    }
}