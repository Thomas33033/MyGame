
//帧同步核心逻辑
using System.Collections;
using System;
using behaviac;

public class LockStepLogic
{
    //累计运行的时间
    float m_totalTime = 0;

    //下一个逻辑帧的时间
    float m_nextFrameTime = 0;

    //预定的每帧的时间长度
    float m_fFrameLen;

    //挂载的逻辑对象
    FightCenter m_callUnit = null;

    //两帧之间的时间差
    float m_fInterpolation = 0;

    public LockStepLogic()
    {
        init();
    }

    public void init()
    {
        m_fFrameLen = (float)FightCenter.g_fixFrameLen;

        m_totalTime = 0;

        m_nextFrameTime = 0;

        m_fInterpolation = 0;
    }

    public void updateLogic()
    {
        float deltaTime = 0;
#if _CLIENTLOGIC_
        deltaTime = UnityEngine.Time.deltaTime;
#else
        deltaTime = 0.1f;
#endif

        m_totalTime = m_totalTime + deltaTime;
        
        while (m_totalTime > m_nextFrameTime)
        {
            m_callUnit.frameLockLogic();

            m_nextFrameTime += m_fFrameLen;

            FightCenter.g_uGameLogicFrame += 1;
        }

        m_fInterpolation = (m_totalTime + m_fFrameLen - m_nextFrameTime) / m_fFrameLen;

        m_callUnit.updateRenderPosition(m_fInterpolation);

    }

    //- 设置调用的宿主
    // @param unit 调用的宿主
    // @return none
    public void setCallUnit(FightCenter unit)
    {
        m_callUnit = unit;
    }

    public float GetTime()
    {
        return this.m_totalTime;
    }
}
