using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFrame
{
    public class CameraFollowTarget
    {
        public CameraController m_cameraCtrl;
        public Transform Target;

        Vector3 m_targetLookAt = Vector3.zero;
        Vector3 m_curLookAt = Vector3.zero;
        Vector3 m_lookAtVelocity = Vector3.zero;

        //附加目标垂直偏移
        public Vector3 AddTragetOffset;
        public EState mState;

        float m_restoreTotalTime = 1;
        float m_restoreBeginTime = 0;

        private float m_currentAngleH; //当前水平角度

        bool m_bhasTargetAngleH = false;  //相机目标水平角度
        float m_beginAngleH = 0;          //相机初始水平角度
        public float m_targetAngleH = 0;
        float m_targetAngleHBeginTime = 0;
        float m_targetAngleHTime = 0.3f;  //水平旋转总时间
        float m_angleVelocityH = 0.0f;    //水平旋转速率

        float m_currentAngleV; //当前垂直角度
        float m_beginAngleV; //开始垂直角度
        public float m_targetAngleV; //目标垂直角度
        bool m_bHasTargetAngleV;
        float m_targetAngleVBeginTime = 0;
        float m_targetAngleVTime = 0;
        float m_angleVelocityV = 0.0f;    //垂直旋转速率

        public float m_maxDistance = 12f;
        public float m_beginDistance = 9.5f;  //开始相机距离
        public float m_targetDistance = 9.5f;  //目标摄像距离
        public float m_currentDistance = 9.5f; //当前摄像距离
        public float m_distanceVelocity = 1f;  //摄像机距离移动速度
        bool m_bHasTargetDistance = true;
        float m_TargetDistanceTime = 0f;
        float m_targetDistanceBeginTime = 0f;


        float m_currentFov;
        float m_beginFov;
        float m_targetFov;

        float m_currentOffsetY;
        float m_beginOffsetY;
        float m_targetOffsetY;

        public bool enableChangeAngleV = true;

        public float MinAngleH = 0;
        public float MaxAngleH = 360;

        public float MinAngleV = 0f;
        public float MaxAngleV = 45f;

        float smoothDampTime = 0.3f;
        public float slipSpeedH = 0.8f;
        public float slipSpeedV = 0.8f;

        public float distMinLimit = 1;
        public float disMaxLimit = 10;
        public float closeUpDistance = 5;

        float m_fPauseTime = 0;
        float m_fPauseTimeBegin = 0;
        bool m_bSmoothToTarget = false;


        public static bool m_enableFollowBack = false;
        public static bool EnableFollowBack
        {
            set
            {
                m_enableFollowBack = value;
            }
            get
            {
                return m_enableFollowBack;
            }
        }

        public void Apply()
        {
            if (Target == null)
                return;
            var heightOffset = 1;

            m_targetLookAt = Target.position + Vector3.up * heightOffset + AddTragetOffset;

            if (mState == EState.Restore)
            {
                RestoreCamera();
                return;
            }

            //刷新水平角度
            UpdateAngleH();
            //刷新垂直角度
            UpdateAngleV();

            if (!m_bhasTargetAngleH) UpdateCameraDistance();

            if (m_cameraCtrl.EnableRotate) RotateCameraBySlipSpeed();
            
            PlayDistanceAnimation();

            if (mState == EState.PauseMove)
            {
                if (Time.time - m_fPauseTimeBegin > m_fPauseTime)
                {
                    mState = EState.none;
                    m_bSmoothToTarget = true;
                }
            }
            else
            {
                FollowTarget();
            }

            m_cameraCtrl.ResetCamera(m_curLookAt, m_currentAngleH, m_currentAngleV, m_currentDistance);
        }

        /// <summary>
        /// 还原相机
        /// </summary>
        private void RestoreCamera()
        {
            float _time = Time.time - m_restoreBeginTime;
            if (_time < m_restoreTotalTime)
            {
                float rate = _time / m_restoreTotalTime;
                m_currentAngleH = CommonTools.SmoothDamp(m_beginAngleH, m_targetAngleH, rate);
                m_currentAngleV = CommonTools.SmoothDamp(m_beginAngleV, m_targetAngleV, rate);
                m_currentDistance = CommonTools.SmoothDamp(m_beginDistance, m_targetDistance, rate);
                m_currentFov = CommonTools.SmoothDamp(m_beginFov, m_targetFov, rate);
                m_currentOffsetY = CommonTools.SmoothDamp(m_beginOffsetY, m_targetOffsetY, rate);
            }
            else
            {
                //摄像机移动完毕后，约束水平角度范围
                m_currentAngleH = CommonTools.AdjustAngle0_t_360(m_targetAngleH);
                m_currentAngleV = m_targetAngleV;
                m_currentDistance = m_targetDistance;
                m_currentFov = m_targetFov;
                m_currentOffsetY = m_targetOffsetY;
                mState = EState.none;
            }

            m_curLookAt = m_targetLookAt;
            m_cameraCtrl.ResetCamera(m_curLookAt, m_currentAngleH, m_currentAngleV, m_currentDistance, m_currentFov, m_currentOffsetY);
        }

        /// <summary>
        /// 更新X轴 水平旋转角度
        /// </summary>
        private void UpdateAngleH()
        {
            if (m_bhasTargetAngleH)
            {   
                //指定时间内过渡
                float _time = Time.time - m_targetAngleHBeginTime;
                if (_time < m_targetAngleHTime)
                {
                    m_currentAngleH = CommonTools.SmoothDamp(m_beginAngleH, m_targetAngleH, _time / m_targetAngleHTime);
                }
                else
                {
                    m_currentAngleV = m_targetAngleH;
                    m_bhasTargetAngleH = false;
                }
            }
            else
            {
                //自然过渡
                if (m_currentAngleH != m_targetAngleH)
                {
                    m_currentAngleH = CommonTools.AdjustAngle0_t_360(m_targetAngleH);
                    Debug.LogError("m_currentAngleH:" + m_currentAngleH);
                   // m_currentAngleH = Mathf.SmoothDampAngle(m_currentAngleH, m_targetAngleH, ref m_angleVelocityH, m_targetAngleHTime);
                }
            }
           
        }

        /// <summary>
        /// 更新Y轴上下旋转
        /// </summary>
        private void UpdateAngleV()
        {
            if (m_bHasTargetAngleV)
            {
                float _time = Time.time - m_targetAngleVBeginTime;
                if (_time < m_targetAngleVTime)
                {
                    m_currentAngleV = CommonTools.SmoothDamp(m_beginAngleV, m_targetAngleV, _time / m_targetAngleVTime);
                }
                else
                {
                    m_currentAngleV = m_targetAngleV;
                    m_bHasTargetAngleV = false;
                }
            }
            else
            {
                if (m_currentAngleV != m_targetAngleV)
                {
                    m_currentAngleV = CommonTools.AdjustAngle0_t_360(m_targetAngleV);
                    m_currentAngleV = Mathf.SmoothDampAngle(m_currentAngleV, m_targetAngleV, ref m_angleVelocityV, m_targetAngleVTime);
                }
            }
        }

        /// <summary>
        /// 旋转相机，根据指定角度
        /// </summary>
        private void RotateCameraBySlipSpeed()
        {
            //相机水平、垂直旋转，效果:选找到边界后自动停止
            if (m_cameraCtrl.RotateDiff != Vector3.zero && m_cameraCtrl.EnableRotate)
            {
                m_currentAngleH += m_cameraCtrl.RotateDiff.x * slipSpeedH;
                m_currentAngleH = CommonTools.AdjustAngle0_t_360(m_currentAngleH);
                m_currentAngleH = Mathf.Clamp(m_currentAngleH, MinAngleH, MaxAngleH);
                if (m_currentAngleH == MinAngleH || m_currentAngleH == MaxAngleH)
                {
                    m_cameraCtrl.RotateDiff.x = 0;
                }

                if (enableChangeAngleV && m_cameraCtrl.RotateDiff.y != 0)
                {
                    m_currentAngleV -= m_cameraCtrl.RotateDiff.y * slipSpeedV;
                    m_currentAngleV = CommonTools.AdjustAngle0_t_360(m_currentAngleV);
                    m_currentAngleV = Mathf.Clamp(m_currentAngleV, MinAngleV, MaxAngleV);
                    if (m_currentAngleV == MinAngleV || m_currentAngleV == MaxAngleV)
                    {
                        m_cameraCtrl.RotateDiff.y = 0;
                    }
                }
            }
        }

       

        /// <summary>
        /// 更新相机距离
        /// </summary>
        private void UpdateCameraDistance()
        {
            if (!m_bhasTargetAngleH)
            {
                float value = m_targetDistance + m_cameraCtrl.ScaleDiff;
                //战斗状态不能缩放摄像机
                //todo
                m_targetDistance = value;
                if (m_cameraCtrl.ScaleDiff < 0 && m_targetDistance < closeUpDistance && CameraController.IsEnterCloseUp)
                {
                    if (this.m_cameraCtrl.EnableCloseUp)
                    {
                        //切换特写状态
                        //m_cameraCtrl.RecordForRestore(m_currentAngleH,m_currentAngleV,closeUpDistance +1);
                        //m_cameraCtrl.change
                        return;
                    }
                    else
                    {
                        m_targetDistance = closeUpDistance;
                    }
                }
                //限制相机距离
                m_targetDistance = Mathf.Clamp(m_targetDistance, closeUpDistance, m_maxDistance);
            }
        }

        /// <summary>
        ///播放相机移动距离动画 
        /// </summary>
        private void PlayDistanceAnimation()
        {
            if (m_bHasTargetDistance)
            {
                //播放相机推进动画
                float _time = Time.time - m_targetDistanceBeginTime;
                if (_time < m_TargetDistanceTime)
                {
                    m_currentDistance = CommonTools.SmoothDamp(m_beginDistance, m_targetDistance, _time / m_TargetDistanceTime);
                }
                else
                {
                    m_currentDistance = m_targetDistance;
                    m_bHasTargetDistance = false;
                }
            }
            else
            {
                //如果相机位置改变，则光滑的移动到目的点
                if (m_currentDistance != m_targetDistance)
                {
                    m_currentDistance = Mathf.SmoothDamp(m_currentDistance, m_targetDistance, ref m_distanceVelocity, smoothDampTime);
                }
            }
        }


        /// <summary>
        /// 相机跟随目标上
        /// 目标点改变时，相机平滑跟随
        /// </summary>
        private void FollowTarget()
        {
            float lookAtDistance = Vector3.Distance(m_curLookAt, m_targetLookAt);
            if (m_bSmoothToTarget)
            {
                if (lookAtDistance < 0.01f)
                {
                    m_bSmoothToTarget = false;
                    m_curLookAt = m_targetLookAt;
                }
                else
                {
                    m_curLookAt = Vector3.SmoothDamp(m_curLookAt, m_targetLookAt, ref m_lookAtVelocity, 0.1f);
                }

            }
            else
            {
                m_curLookAt = m_targetLookAt;
            }
        }

    }


    public enum EState
    {
        none,
        Restore,
        PauseMove,
    }


    class CommonTools
    {
        public static float SmoothDamp(float start, float end, float normalizedTime)
        {
            float t = Mathf.Clamp(normalizedTime, 0, 1);
            return (t * t * t - 3 * t * t + 3 * t) * (end - start) + start;
        }

        public static float AdjustAngle0_t_360(float angle)
        {
            while (angle < 0f)
            {
                angle += 360f;
            }

            while (angle >= 360)
            {
                angle -= 360;
            }

            return angle;
        }
    }
}