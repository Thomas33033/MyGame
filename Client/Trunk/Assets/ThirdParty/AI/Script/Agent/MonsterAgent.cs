﻿//// -------------------------------------------------------------------------------
//// THIS FILE IS ORIGINALLY GENERATED BY THE DESIGNER.
//// YOU ARE ONLY ALLOWED TO MODIFY CODE BETWEEN '///<<< BEGIN' AND '///<<< END'.
//// PLEASE MODIFY AND REGENERETE IT IN THE DESIGNER FOR CLASS/MEMBERS/METHODS, ETC.
//// -------------------------------------------------------------------------------

//using behaviac;
//using System;
//using System.Collections;
//using System.Collections.Generic;

/////<<< BEGIN WRITING YOUR CODE FILE_INIT

/////<<< END WRITING YOUR CODE

//public class MonsterAgent : behaviac.Agent
/////<<< BEGIN WRITING YOUR CODE MonsterAgent
/////<<< END WRITING YOUR CODE
//{
//    ///<<< BEGIN WRITING YOUR CODE CLASS_PART

//    ////Monster monster;
//    ////MoveComponent mMoveComponent;
//    ////ScanTargetComponent mScanTargetComponent;
//    ////BufferStateComponent mBuffserStateComponent;

//    public void InitAgent(Monster p_monster)
//    {
//        monster = p_monster;
//        mMoveComponent = p_monster.GetComponent<MoveComponent>();
//        mScanTargetComponent = p_monster.GetComponent<ScanTargetComponent>();
//        mBuffserStateComponent = p_monster.GetComponent<BufferStateComponent>();
//    }

//    //开始巡逻
//    public EBTStatus StartPatrol()
//    {
//        bool reached = mMoveComponent.ArrivedAtTarget();
//        if (reached)
//        {
//            mMoveComponent.StartPatrol();

//            return behaviac.EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_RUNNING;
//        }
//    }

//    //向目标移动
//    public EBTStatus MoveToTarget()
//    {
//        if (null != mScanTargetComponent.Target)
//        {
//            if (!mScanTargetComponent.CheckArrivedTargetPosition())
//            {
//                mMoveComponent.MoveToTarget(mScanTargetComponent.Target.ClientPos);
//                return EBTStatus.BT_SUCCESS;
//            }
//            else
//            {
//                return EBTStatus.BT_RUNNING;
//            }
//        }

//        return EBTStatus.BT_INVALID;
//    }

//    //视野内是否有目标
//    public EBTStatus SeekTarget()
//    {
//        if (null != mScanTargetComponent.Target)
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        //重新扫描目标
//        mScanTargetComponent.ScanForTarget();
//        if (null != mScanTargetComponent.Target)
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }

//    //检查目标是否可以攻击
//    public EBTStatus CanAttack()
//    {
//        if (mScanTargetComponent.Target != null && monster.CanAttack(mScanTargetComponent.Target))
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }


//    //攻击目标
//    public EBTStatus AttactTarget()
//    {
//        return EBTStatus.BT_INVALID;
//    }

//    /// <summary>
//    /// 已经死亡
//    /// </summary>
//    /// <returns></returns>
//    public EBTStatus HasDied()
//    {
//        if (mBuffserStateComponent.HasState(StateType.Death))
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }


//    //目标是否在攻击范围内
//    public EBTStatus TargetInAttactRange()
//    {
//        if (mScanTargetComponent.Target != null && monster.TargetInAttackRange(mScanTargetComponent.Target))
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }
//    /// <summary>
//    /// 愤怒状态已满
//    /// </summary>
//    /// <returns></returns>
//    public EBTStatus IsAngry()
//    {
//        if (monster.IsAngry())
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }

//    /// <summary>
//    /// 激活死亡状态
//    /// </summary>
//    /// <returns></returns>
//    public EBTStatus EnableDieState()
//    {
//        if (monster.HP <= 0 && mBuffserStateComponent.HasState(StateType.Death))
//        {
//            monster.OnDie();
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }

//    /// <summary>
//    /// 激活复活状态
//    /// </summary>
//    /// <returns></returns>
//    public EBTStatus EnableReviveState()
//    {
//        return EBTStatus.BT_INVALID;
//    }

//    /// <summary>
//    /// 目标消失
//    /// </summary>
//    /// <returns></returns>
//    public EBTStatus TargetVanish()
//    {
//        if (this.mScanTargetComponent.Target == null)
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else if (this.CanAttack() == EBTStatus.BT_INVALID)
//        {
//            return EBTStatus.BT_SUCCESS;
//        }
//        else
//        {
//            return EBTStatus.BT_INVALID;
//        }
//    }



//    ///<<< END WRITING YOUR CODE

//}

/////<<< BEGIN WRITING YOUR CODE FILE_UNINIT

/////<<< END WRITING YOUR CODE

