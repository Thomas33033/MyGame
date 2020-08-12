using DG.Tweening;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoSingleton<CameraController>
{
    [HideInInspector]
    public Camera mainCamera;

    public Transform followTarget;

    public GameObject TouchTarget;

    public BoxCollider boxCollider;

    public float momentumAmount = 35f;

    bool mPressed = false;

    bool mDragStarted = false;

    public Vector2 scale = new Vector2(0.02f, 0);

    public Vector2 overstepDis;

    bool smoothDragStart;

    bool beginDrag;

    float smoothTime = 0.04f;

    Vector3 velocity = Vector3.zero;

    private bool isMoving = false;

    private LayerMask dragMask;

    public bool dragEnable = true;

    public bool followEnable = true;

    //----------------new start-------------------
    public float distance;
    public float x;
    public float y;

    private float lastDistance, lastX, lastY;

    public float orbitDamping = 4.0f;
    private Vector3 targetPosition;

    public Vector3 minPox;
    public Vector3 maxPox;

    private Vector3 lastPos;
    private Vector3 dragPos;
    
    public Vector3 lastFollowPos = new Vector3(-1, 0, 0);

    private Vector3 _min, _max;

    private Vector3 b1, b2, b3, b4;

    protected Vector3 mMomentum = Vector3.zero;


    //----------------new end-------------------

    private Vector3[] corners = new Vector3[5];

    private List<Vector2> ScreenCornerPosList = new List<Vector2> {
            Vector2.zero
            , new Vector2(0, Screen.height)
            , new Vector2(Screen.width, Screen.height)
            , new Vector2(Screen.width, 0)
            , new Vector2(Screen.width/2, Screen.height/2)
    };



    public void Awake()
    {
        dragMask = 1 << LayerMask.NameToLayer("Terrain");
        mainCamera = GetComponent<Camera>();

        EasyTouch.AddCamera(mainCamera);

        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        _min = boxCollider.bounds.min;//АќЮЇКа
        _max = boxCollider.bounds.max;

        b1 = new Vector3(_min.x, _min.y, _max.z);
        b2 = _max;
        b3 = new Vector3(_max.x, _min.y, _min.z);
        b4 = _min;
    }


    public void OnEnable()
    {
        lastFollowPos = new Vector3(-1, 0, 0);
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

    private void OnDestroy()
    {
        EasyTouch.On_Drag -= OnDrag;
        EasyTouch.On_DragStart -= OnDragStart;
        EasyTouch.On_DragEnd -= OnDragEnd;
        EasyTouch.RemoveCamera(mainCamera);
    }


    void Update()
    {
#if UNITY_EDITOR
        _min = boxCollider.bounds.min;//АќЮЇКа
        _max = boxCollider.bounds.max;
#endif

        if (!mPressed && dragEnable && !beginDrag)
        {
            if (this.isMoving)
            {
                //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                //if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                //{
                //    transform.position = targetPosition;
                //    isMoving = false;
                //}
            }
            else
            {
                if (followEnable)
                {
                    if (lastFollowPos != followTarget.position || lastDistance != distance || lastY != y || lastX != x)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
                        Vector3 pos = transform.rotation * new Vector3(0, 0, -distance) + followTarget.position;
                        ConstrainToBounds(pos, true, overstepDis);
                        lastFollowPos = followTarget.position;
                        lastDistance = distance;
                        lastY = y;
                        lastX = x;
                    }
                }

                if (mMomentum.magnitude > 0.01f)
                {
                    transform.position += (Vector3)SpringDampen(ref mMomentum, 9f, Time.deltaTime);
                    if (!ConstrainToBounds(transform.position, true, Vector2.zero))
                    {
                    }
                    return;
                }

            }
        }
        else
        {
            SpringDampen(ref mMomentum, 9f, Time.unscaledDeltaTime);
        }
    }

    Vector3 SpringDampen(ref Vector3 velocity, float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        float dampeningFactor = 1f - strength * 0.001f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        float totalDampening = Mathf.Pow(dampeningFactor, ms);
        Vector3 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
        velocity = velocity * totalDampening;
        return vTotal * 0.06f;
    }

    public void OnDragStart(Gesture gesture)
    {
        if (!dragEnable || isMoving) return;
        if ((gesture.pickedObject == TouchTarget) && gesture.fingerIndex == 0)
        {
            mPressed = beginDrag = true;
            lastPos = GetWorldPosition(gesture.position);
            lastPos.y = 0;
        }
    }
    public void OnDrag(Gesture gesture)
    {
        if (isMoving) return;
        if (dragEnable && beginDrag)
        {
            Drag(gesture.position);

        }
    }

    public void OnDragEnd(Gesture gesture)
    {
        if (isMoving) return;
        if (dragEnable && beginDrag)
        {
            beginDrag = false;
            mPressed = false;

            dragPos = GetWorldPosition(gesture.position);
            dragPos.y = 0;
            Vector3 offset = lastPos - dragPos;
            mMomentum +=  offset * 2;

            mMomentum.x = Mathf.Clamp(mMomentum.x, -1, 1);
            mMomentum.z = Mathf.Clamp(mMomentum.z, -1, 1);
            ConstrainToBounds(transform.position, false, Vector2.zero);
        }
    }

    public void Drag(Vector2 postion)
    {
        if (smoothDragStart && !mDragStarted)
        {
            mDragStarted = true;
            return;
        }
        dragPos = GetWorldPosition(postion);
        dragPos.y = 0;
        Vector3 offset = lastPos - dragPos;

        Vector3 pos = transform.position + offset;

        if (ConstrainToBounds(pos, true, overstepDis))
        {
            mMomentum = Vector2.zero;
        }

    }


    Vector3 GetWorldPosition(Vector3 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100f, dragMask))
        {
            return hitInfo.point;
        }
        return Vector3.zero;
    }


    private float _minX, _maxX, _minZ, _maxZ;
    private bool ConstrainToBounds(Vector3 targePos, bool immediate, Vector2 overstep)
    {
        Vector3[] corners = FindLowerCorners();
        float width = Mathf.Abs(corners[1].x - corners[2].x);

        float height = Mathf.Abs(corners[0].z - corners[1].z);

        Vector3 center = Vector3.zero;
        center.x = (corners[1].x + corners[2].x) / 2;
        center.z = (corners[0].z + corners[1].z) / 2;

        float cameraZDis = Mathf.Abs(center.z - this.transform.position.z);

        _minX = _min.x + width / 2;
        _maxX = _max.x - width / 2;
        _minZ = _min.z + height / 2 - cameraZDis;
        _maxZ = _max.z - height / 2 - cameraZDis;

        Vector3 offset = Vector3.zero;

        if (targePos.x < _minX - overstep.x)
        {
            offset.x = targePos.x - _minX + overstep.x;
        }
        else if (targePos.x > _maxX + overstep.x)
        {
            offset.x = targePos.x - _maxX - overstep.x;
        }

        if (targePos.z < _minZ - overstep.y)
        {
            offset.z = targePos.z - _minZ + overstep.y;
        }
        else if (targePos.z > _maxZ + overstep.y)
        {
            offset.z = targePos.z - _maxZ - overstep.y;
        }

        if (offset.sqrMagnitude > 0f)
        {
            targePos -= offset;
            if (immediate)
            {
                this.transform.position = targePos;
            }
            else
            {
                transform.DOMove(transform.position - offset, 0.5f);
                mMomentum = Vector2.zero;
            }
            return true;
        }
        else
        {
            this.transform.position = targePos;
        }

        return false;
    }


    Vector3[] FindLowerCorners()
    {
        for (int k = 0; k < ScreenCornerPosList.Count; k++)
        {
            Ray ray = mainCamera.ScreenPointToRay(ScreenCornerPosList[k]);
            var hits = Physics.RaycastAll(ray, 1000, dragMask);
            if (hits == null || hits.Length <= 0)
            {
                continue;
            }
            for (var i = 0; i < hits.Length; ++i)
            {
                if (hits[i].transform.gameObject == this.TouchTarget.gameObject)
                {
                    corners[k] = hits[i].point;
                    break;
                }
            }
        }

        return corners;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_min, _max);


        Gizmos.DrawLine(b1, b2);
        Gizmos.DrawLine(b2, b3);
        Gizmos.DrawLine(b3, b4);
        Gizmos.DrawLine(b4, b1);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[0], corners[3]);
        Gizmos.DrawLine(corners[3], corners[2]);
        Gizmos.DrawLine(corners[2], corners[1]);



    }
}






