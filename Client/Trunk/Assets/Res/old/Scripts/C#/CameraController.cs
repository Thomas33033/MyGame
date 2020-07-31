using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    public Transform dragTarget;
    public Camera mainCamera;
    public BoxCollider Bounds;


    private Vector3 _min, _max;

    public bool EnableMove = false;//是否需要移动

    private bool beginDrag = false;
    private Vector3 LastPosition = Vector3.zero;

    bool mPressed = false;

    private Vector3 lastPos;
    private Vector3 dragPos;

    private LayerMask canDragMask;

    public float distance;

    public Vector3[] corners = new Vector3[5];

    private List<Vector2> ScreenCornerPosList = new List<Vector2> {
            Vector2.zero
            , new Vector2(0, Screen.height)
            , new Vector2(Screen.width, Screen.height)
            , new Vector2(Screen.width, 0)
            , new Vector2(Screen.width/2, Screen.height/2)
    };


    private void Awake()
    {

    }

    public void Start()
    {
        canDragMask = 1 << LayerMask.NameToLayer("Terrain");

        _min = Bounds.bounds.min;//包围盒
        _max = Bounds.bounds.max;
        EasyTouch.AddCamera(mainCamera);
        distance = Vector3.Distance(this.mainCamera.transform.position, this.dragTarget.position);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_min, _max);

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[0], corners[3]);
        Gizmos.DrawLine(corners[3], corners[2]);
        Gizmos.DrawLine(corners[2], corners[1]);

        Gizmos.DrawLine(this.mainCamera.transform.position, corners[4]);
        
    }

    private void OnDestroy()
    {
        EasyTouch.RemoveCamera(mainCamera);
        LastPosition = transform.position;
    }


    private void OnEnable()
    {
        EasyTouch.On_Drag += OnDrag;
        EasyTouch.On_DragStart += OnDragStart;
        EasyTouch.On_DragEnd += OnDragEnd;
    }

    private void OnDisable()
    {
        EasyTouch.On_Drag -= OnDrag;
        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_DragEnd -= OnDragEnd;
    }


    private void OnTouchStart(Gesture gesture)
    {
        Debug.LogError("OnTouchStart");
    }

    private void OnTouchDown(Gesture gesture)
    {
        Debug.LogError("OnTouchDown");
    }

    public void OnDragStart(Gesture gesture)
    {
        if (gesture.fingerIndex == 0 && !EnableMove)
        {
            mPressed = beginDrag = true;
            lastPos = GetWorldPosition(gesture.position);
            lastPos.y = 0;
        }

    }

    public void OnDragEnd(Gesture gesture)
    {
        if (beginDrag)
        {
            mPressed = false;
            //ConstrainToBounds(false);
            beginDrag = false;
        }
    }

    private void ConstrainToBounds(Vector3 pos)
    {
        //正交摄像机 
        //var cameraHalfWidth = mainCamera.orthographicSize * ((float)Screen.width / Screen.height);
        ////保证不会移除包围盒
        //pos.x = Mathf.Clamp(pos.x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
        //pos.z = Mathf.Clamp(pos.z, _min.z + mainCamera.orthographicSize, _max.z - mainCamera.orthographicSize);

        //透视摄像机
        //Vector3[] corners = GetCorners(distance);
        Vector3[] corners = FindLowerCorners();

        float width = Mathf.Abs(corners[1].x - corners[2].x);

        float height = Mathf.Abs(corners[0].z - corners[1].z);

        Vector3 center = Vector3.zero;
        center.x = (corners[1].x + corners[2].x) / 2;
        center.z = (corners[0].z + corners[1].z) / 2;

        float cameraZDis = Mathf.Abs(center.z - this.transform.position.z);

        pos.x = Mathf.Clamp(pos.x, _min.x + width / 2, _max.x - width / 2);
        pos.z = Mathf.Clamp(pos.z, _min.z + height / 2 - cameraZDis, _max.z - height / 2 - cameraZDis);

        transform.position = pos;
    }


    public void OnDrag(Gesture gesture)
    {
        if (beginDrag && gesture.fingerIndex == 0)
        {
            dragPos = GetWorldPosition(gesture.position);
            dragPos.y = 0;
            ConstrainToBounds(mainCamera.transform.position + lastPos - dragPos);
        }
    }

    /// <summary>
    /// 检查是否点击到cbue
    /// </summary>
    /// <returns></returns>
    Vector3 GetWorldPosition(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100f, canDragMask))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    

    Vector3[] FindLowerCorners()
    {
        for (int k = 0; k < ScreenCornerPosList.Count; k++)
        {
            Ray ray = mainCamera.ScreenPointToRay(ScreenCornerPosList[k]);
            var hits = Physics.RaycastAll(ray, 1000, canDragMask);
            if (hits == null || hits.Length <= 0)
            {
                continue;
            }
            for (var i = 0; i < hits.Length; ++i)
            {
                if (hits[i].transform.gameObject == this.dragTarget.gameObject)
                {
                    corners[k] = hits[i].point;
                    break;
                }
            }

        }



        //viewWidth = Vector3.Distance(corners[0], corners[1]);
        //viewHeight = Vector3.Distance(corners[0], corners[2]);
        // for debugging
   
        return corners;
    }



    //Vector3[] GetCorners(float distance)
    //{

    //    Transform tx = mainCamera.transform;

    //    float halfFOV = (mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
    //    float aspect = mainCamera.aspect;

    //    float height = distance * Mathf.Tan(halfFOV);
    //    float width = height * aspect;

    //    // UpperLeft
    //    corners[0] = tx.position - (tx.right * width);
    //    corners[0] += tx.up * height;
    //    corners[0] += tx.forward * distance;

    //    // UpperRight
    //    corners[1] = tx.position + (tx.right * width);
    //    corners[1] += tx.up * height;
    //    corners[1] += tx.forward * distance;

    //    // LowerLeft
    //    corners[2] = tx.position - (tx.right * width);
    //    corners[2] -= tx.up * height;
    //    corners[2] += tx.forward * distance;

    //    // LowerRight
    //    corners[3] = tx.position + (tx.right * width);
    //    corners[3] -= tx.up * height;
    //    corners[3] += tx.forward * distance;

    //    return corners;
    //}

    //public bool ConstrainToBounds(Vector3 targePos, bool immediate, float overstep = 0)
    //{
    //    Vector3 offset = Vector3.zero;

    //    if (targePos.x < minPox.x - overstep)
    //    {
    //        offset.x = targePos.x - minPox.x + overstep;
    //    }
    //    if (targePos.x > maxPox.x + overstep)
    //    {
    //        offset.x = targePos.x - maxPox.x - overstep;
    //    }

    //    if (targePos.z < minPox.z - overstep)
    //    {
    //        offset.z = targePos.z - minPox.z + overstep;
    //    }
    //    if (targePos.z > maxPox.z + overstep)
    //    {
    //        offset.z = targePos.z - maxPox.z - overstep;
    //    }

    //    if (offset.sqrMagnitude > 0f)
    //    {
    //        targePos -= offset;
    //        if (immediate)
    //        {
    //            this.transform.position = targePos;
    //        }
    //        else
    //        {
    //            targetPosition = targePos;
    //        }
    //        return true;
    //    }

    //    return false;
    //}
}
