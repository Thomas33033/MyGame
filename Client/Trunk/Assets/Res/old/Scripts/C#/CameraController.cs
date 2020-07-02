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

    private LayerMask canDrag;

    public float distance;

    private void Awake()
    {
    }

    public void Start()
    {
        canDrag = 1 << LayerMask.NameToLayer("Terrain");

        _min = Bounds.bounds.min;//包围盒
        _max = Bounds.bounds.max;
        EasyTouch.AddCamera(mainCamera);
        distance = Vector3.Distance(this.mainCamera.transform.position, this.dragTarget.position);
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
        //float width = Vector3.Distance(corners[0], corners[1]);
        //float height = Vector3.Distance(corners[0], corners[2]);
        //pos.x = Mathf.Clamp(pos.x, _min.x + width / 2, _max.x - width / 2);
        //pos.z = Mathf.Clamp(pos.z, _min.z + height / 2, _max.z - height / 2);

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
        if (Physics.Raycast(ray, out hitInfo, 100f, canDrag))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }


    Vector3[] GetCorners(float distance)
    {
        Vector3[] corners = new Vector3[4];
        Transform tx = mainCamera.transform;

        float halfFOV = (mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = mainCamera.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        return corners;
    }
}
