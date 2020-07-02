using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace GameFrame
{

    public class CameraController_2 : MonoBehaviour
    {
        public static float DefaultAngleH = 0;
        public static float DefaultAngleV = 0;
        public static float DefaultDistance = 9.5f;
        public static float DefaultFov = 50f;
        public static float DefaultOffsetY = 0f;

        float currentOffsetY = DefaultOffsetY;
        bool m_recorded = false;
        float m_recordAngleH = 0f;
        float m_recordAngleV = 0f;
        float m_recordDistance = 0f;
        float m_recordFov = 50f;
        float m_recordOffsetY = 0f;

        public Vector3 currentLookAt = Vector3.zero;
        public float CurrentAngleH = DefaultAngleH;
        public float CurrentAngleV = DefaultAngleV;
        public float currentDistance = DefaultDistance;
        public float CurrentOffsetY = DefaultOffsetY;

        public static bool isCommonExistCloseUp = true;
        public static bool IsEnterCloseUp = true;  //是否可以进入特写
        public bool allowLissThanCloseUpDis = true; //允许不在特写中时小于进入特写距离

        //激活特写状态
        public bool EnableCloseUp = false;

        float m_lastTouchDistance = 0.0f;  //记录两指间距离
        float m_scaleSpeed = 10f;          //摄像机放大缩小速度
        float m_scaleSpeedPhone = 0.5f;

        float _currentFov = DefaultFov;
        public Camera CurrentCamera = null;

        float Angle = 0f;
        Vector2 cameraForward;

        public bool EnableCameraCollider = false;

        public float ScaleDiff = 0f;
        public Vector3 RotateDiff = Vector3.zero;
        public bool EnableRotate = true;

        //是否进入特写
        public bool InCloseUp
        {
            get
            {
                return true;
            }
        }

        //场景锁
        public static bool m_sceneLock = false;
        public static bool EnableSceneLock
        {
            set
            {
                m_sceneLock = value;
            }
            get
            {
                return m_sceneLock;
            }
        }

        public void UpdateAngle()
        {
            Angle = Vector3.Angle(Vector2.up, cameraForward);
            if (cameraForward.x < 0)
            {
                Angle = 360 - Angle;
            }
        }

        Vector2 _cameraForward;
        public Vector2 CameraForward
        {
            get
            {
                return _cameraForward;
            }
        }

        private CameraFollowTarget camereFollow;

        //--------------------------UI测试--------------------------------------
        public Button ChangeAngleBtn;
        //////////////////////////////////////////////////////////////////

        void Awake()
        {
            this.camereFollow = new CameraFollowTarget();
            this.camereFollow.m_cameraCtrl = this;
            this.camereFollow.Target = this.Target;
        }

        void Update()
        {
            this.ZoomCamera();
            this.camereFollow.Apply();
        }

        public Transform m_target = null;
        public Transform Target
        {
            set
            {
                if (m_target != value)
                {
                    Transform _oldTaget = m_target;
                    m_target = value;

                    if (_oldTaget != null && m_target != null && Vector3.Distance(_oldTaget.position, m_target.position) < 5)
                    {
                        //SmoonthFollowTarget(true);
                    }
                }
            }
            get
            {
                return m_target;
            }
        }

        public void ResetCamera(Vector3 lookAt, float angleH, float angleV, float distance, float fov = 0, float offsety = 0)
        {
            if (fov == 0)
            {
                fov = DefaultFov;
            }

            if (offsety == 0)
            {
                offsety = currentOffsetY;
            }


            if (lookAt == currentLookAt
                && angleH == CurrentAngleH
                && angleV == CurrentAngleV
                && distance == currentDistance
                && fov == _currentFov
                && offsety == CurrentOffsetY)
            {
                return;
            }

            //设置摄像机的位置
            lookAt = lookAt + offsety * Vector3.up;
            Quaternion currentRotation = Quaternion.Euler(0, angleH, 0);
            Vector3 cameraPos = lookAt;
            cameraPos += currentRotation * Vector3.back * (Mathf.Cos(angleV * Mathf.Deg2Rad) * distance);
            cameraPos.y += Mathf.Sin(angleV * Mathf.Deg2Rad) * distance;
            this.transform.position = cameraPos;
            this.transform.LookAt(lookAt);
            CurrentCamera.fieldOfView = fov;

            //摄像机碰撞检测
            if (this.InCloseUp || EnableSceneLock)
            {
                RaycastHit hit;
                if (Physics.Raycast(lookAt, -this.transform.forward, out hit, distance))
                {
                    if (hit.transform.gameObject.layer == Const1.Layer.Terrain)
                    {
                        this.transform.position = hit.point + this.transform.forward * 0.2f;
                    }
                }
            }
            else
            {
                bool _resetPositionByHit = false;
                RaycastHit hit;
                if (Physics.Raycast(lookAt, -this.transform.forward, out hit, distance))
                {
                    if (hit.transform.gameObject.layer == Const1.Layer.Default
                        || hit.transform.gameObject.layer == Const1.Layer.Terrain)
                    {
                        try
                        {
                            if (hit.transform != null && hit.transform.CompareTag("CameraCollider"))
                            {
                                if (this.transform != null)
                                {
                                    this.transform.position = hit.point + this.transform.forward * 0.2f;
                                    _resetPositionByHit = true;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 手机平台触摸，相机缩放 相机旋转
        /// </summary>
        private void MobileTouch()
        {
            
            if (Const1.IsPointOverUIObject())
            {
                return;
            }

            if (Input.touchCount < 2)
            {
                m_lastTouchDistance = 0;
            }

            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    m_lastTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if ((touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) && m_lastTouchDistance > 0)
                {
                    //触发缩放
                    float touchTempDist = Vector2.Distance(touch1.position, touch2.position);
                    ScaleDiff = -(touchTempDist - m_lastTouchDistance) * m_scaleSpeed / Screen.width;
                    m_lastTouchDistance = touchTempDist;
                    //摄像机缩放过程中，不能旋转
                    RotateDiff = Vector3.zero;
                }
            }
        }

        float xSpeed = 5f;
        float ySpeed = 2.5f;

        private void MouseCamera()
        {
            Vector2 mousePosition = Input.mousePosition;
            if (mousePosition.x < 0 || mousePosition.y < 0
                || mousePosition.x >= Screen.width || mousePosition.y >= Screen.height)
            {
                return;
            }

            if (!Const1.IsPointOverUIObject())
            {
                ScaleDiff = Input.GetAxis("Mouse ScrollWheel") * (-m_scaleSpeed);
            }
            

            if (Input.GetMouseButton(0))
            {
                Cursor.visible = true;
                this.camereFollow.m_targetAngleH += Input.GetAxis("Mouse X") * xSpeed;
                this.camereFollow.m_targetAngleV -= Input.GetAxis("Mouse Y") * ySpeed;
            }
        }

        //相机缩放
        public void ZoomCamera()
        {
           MouseCamera();
           // MobileTouch();
        }
    }
  
}
class Const1
{
    public struct Layer
    {
        public static int Terrain = LayerMask.NameToLayer("Terrain");
        public static int Default = LayerMask.NameToLayer("Default");
    }

    public static bool IsPointOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}