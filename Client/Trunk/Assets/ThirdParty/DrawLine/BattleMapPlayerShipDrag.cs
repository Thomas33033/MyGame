using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleMapPlayerShipDrag : MonoBehaviour
{
    public Vector3[] wpPos;

    List<Vector3> path = new List<Vector3>();

    Vector3 previousPosition, oldPosition;

    public float minDis = 0.1f, maxDis = 0.2f;

    public GameObject touch;

    //public GameObject sea;
    bool startDrag = false/*, hitIsland = false*/;
    
    private LayerMask mask;

    public int maxPointNum = 1000;

    //public ShipMove shipMove;

    public BattleMapShipDragLine customLine;

    private float minDragDis = 2.6f;

    void Start()
    {
        mask = 1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Default");


        touch.transform.position = Vector3.one * int.MaxValue;
        //shipMove.reachedNetxPoint+=ReachedNetxPoint;
        //shipMove.reachedEndxPoint+=ReachedEnd;

#if UNITY_EDITOR
        minDragDis=0;
#endif
    }

    IEnumerator run()
    {
        while (true)
        {
            if (customLine.points.Count>1)
            {
                Move();
                break;
            }
            yield return 0;
        }

    }

    private void ReachedNetxPoint()
    {
        customLine.DelStart();
        if (path.Count>0) path.RemoveAt(0);
        GetWayPos();
    }

    private void ReachedEnd()
    {
        if (customLine.points.Count>0)
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    public void Move()
    {
        GetWayPos();
        path.Clear();
        path.AddRange(wpPos);

        //shipMove.Move(wpPos);
    }

    void GetWayPos()
    {
        wpPos=customLine.points.ToArray();
    }

    public void Stop()
    {
        touch.transform.position = Vector3.one * int.MaxValue;
        startDrag = false;
        customLine.Clear();
        //shipMove.startMove=false;
        //shipMove.SetAngle();
        //shipMove.ClearPath();
    }

    public void Pause()
    {
        startDrag = false;
        //shipMove.startMove = false;
        //shipMove.SetAngle();
    }

    public void Resume()
    {
        //if (shipMove.startMove) return;
        startDrag = false;
        //shipMove.startMove = true;
    }

    void OnEnable()
    {
        EasyTouch.On_DragStart += On_DragStart;
        EasyTouch.On_DragEnd += On_DragEnd;
        EasyTouch.On_Drag += On_Drag;
        EasyTouch.On_SimpleTap += On_SimpleTap;
    }

    void OnDestroy()
    {
        EasyTouch.On_DragStart -= On_DragStart;
        EasyTouch.On_DragEnd -= On_DragEnd;
        EasyTouch.On_Drag -= On_Drag;
        EasyTouch.On_SimpleTap -= On_SimpleTap;
    }

    public void OnDisable()
    {
        EasyTouch.On_DragStart -= On_DragStart;
        EasyTouch.On_DragEnd -= On_DragEnd;
        EasyTouch.On_Drag -= On_Drag;
        EasyTouch.On_SimpleTap -= On_SimpleTap;
    }

    void On_SimpleTap(Gesture gesture)
    {
        if (gesture.pickedObject != null)
        {
            if (gesture.pickedObject == gameObject)
            {
                Stop();
            }
            else
            {
                //SoundMgr.Instance().PlaySound("SE_hint_04");
                //SceneMgr.Instance().GetCurrentScene().Notification(SceneEventEnum.ShowTaskPoint, this, null);
            }

        }
    }

    void On_Drag(Gesture gesture)
    {
        if (startDrag && gesture.fingerIndex == 0)
		{
            Vector3 position = GetTouchToWorldPoint(gesture.position);
            AddPoint(previousPosition, ref position);

            if (path.Count > maxPointNum)
            {
                startDrag = false;
                return;
            }
        }
    }

	void On_DragStart(Gesture gesture)
	{
        if (gesture.pickedObject != null && gesture.fingerIndex == 0)
		{
			Vector3 position = GetTouchToWorldPoint(gesture.position);

			if (gesture.pickedObject == gameObject)
			{
                Debug.LogError("On_DragStart self");
                RaycastHit2D hit2D = Physics2D.Raycast(transform.position, position - transform.position, (position - transform.position).magnitude, mask.value, 0);
				if (hit2D.collider != null && hit2D.collider.tag == "Island")
				{
					return;
				}
				previousPosition = position = transform.position;

				customLine.Clear();
                wpPos = customLine.points.ToArray();
				path.Clear();
				LineAddPoint(position);
				startDrag = true;
                
				StartCoroutine(run());
			}
			else if (gesture.pickedObject == touch)
			{
				RaycastHit2D hit2D = Physics2D.Raycast(touch.transform.position, position - touch.transform.position, (position - touch.transform.position).magnitude, mask.value, 0);
				if (hit2D.collider != null && hit2D.collider.tag == "Island")
				{
					return;
				}
				//if (!hitIsland)
				{
					position = touch.transform.position; 
					//LineAddPoint(position);
					previousPosition = position;
				}
				startDrag = true;
				//hitIsland = false;
			}

			if (startDrag)
			{
				//TODO
				//BattleMap.instance.ship.actoMove = false;
			}

		}
	}

	void On_DragEnd(Gesture gesture)
	{
		if (startDrag && gesture.fingerIndex == 0)
		{
			startDrag = false;
			//startMove = true;
		}
	}

    void AddPoint(Vector3 p1, ref Vector3 p2)
    {
        //hitIsland = true;
        RaycastHit2D[] hit2D = Physics2D.LinecastAll(p1, p2, mask.value, 0);
        
        for (int i = 0; i < hit2D.Length; ++i)
        {
            if (hit2D[i].collider.tag == "Island")
            {
                if (Vector3.Distance(hit2D[i].point, p1) <= minDis)
                {
                    return;
                }
                else
                {
                    LineAddPoint((Vector3)hit2D[i].point + (p1 - p2).normalized * minDis);
                    p2 = (Vector3)hit2D[i].point + (p1 - p2).normalized * minDis;
                    touch.transform.position = p2 + Vector3.back * p2.z;
                }
                return;
            }
        }
        touch.transform.position = p2 + Vector3.back * p2.z;
        LineAddPoint(p2);
        //hitIsland = false;
    }

    void LineAddPoint(Vector3 point)
    {
        if (path.Count==0)
        {
            previousPosition = transform.position;
            path.Add(previousPosition);
            customLine.AddPoint(previousPosition);
        }
        if (path.Count == 1)
        {
            if (Vector3.Distance(path[0], point) > minDragDis)
            {
                point = point - point.z * Vector3.forward;
                previousPosition = point;

                path.Add(point);
                customLine.AddPoint(point);
            }
        }
        else
        {
            point = point - point.z * Vector3.forward;
            previousPosition = point;

            path.Add(point);
            customLine.AddPoint(point);
        }
    }

    public Vector3 GetTouchToWorldPoint(Vector3 position3D)
    {
        Camera camera = Camera.main;
        float depthZ = -camera.transform.position.z;
        Vector3 position = camera.ScreenToWorldPoint(new Vector3(position3D.x, position3D.y, depthZ));
        float angleX = Mathf.Atan((position.x - camera.transform.position.x) / (position.z - camera.transform.position.z)) * Mathf.Rad2Deg;
        float angleY = Mathf.Atan((position.y - camera.transform.position.y) / (position.z - camera.transform.position.z)) * Mathf.Rad2Deg;
        position.x = depthZ * Mathf.Tan(angleX * Mathf.Deg2Rad) + camera.transform.position.x;
        position.y = depthZ * Mathf.Tan(angleY * Mathf.Deg2Rad) + camera.transform.position.y;
        position.z = 0;
        return position;
    }

}
