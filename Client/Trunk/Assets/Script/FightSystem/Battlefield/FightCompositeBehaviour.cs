using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Fight
{
    /// <summary>
    /// 战斗组合行为基类
    /// 1.初始化战场
    /// 2.播放战报
    /// 3.暂停、开始战场
    /// 战场行为子类为：实时战场、战报重播 两种
    /// </summary>
    public class FightCompositeBehaviour
    {
        public BattleComposite composite;

        protected FightData _fightData;

        public int owerId { get => _fightData.selfBattleData.userData.userID; }

        protected List<FightReport> _listReport;

        protected FightType _fightType;

        public delegate void ReciveFightReportHandler(FightReport report);

        public event ReciveFightReportHandler reciveEvent;

        public virtual bool isFight => false;

        public FightCompositeBehaviour()
        {
            _listReport = new List<FightReport>();
        }

        private void OnDestroy()
        {
            reciveEvent = null;
        }

        public virtual float GetTime()
        {
            return 0;
        }

        //public void ReciveEventInvoke(FightReport fightReport)
        //{
        //    this.reciveEvent.Invoke(fightReport);
        //}

        public virtual void InitFight(FightType fightType, FightData fightData)
        {
            _fightData = fightData;
            _fightType = fightType;
        }

        public virtual void SetAutoSkill(bool v)
        {
        }

        public virtual void StartBattle()
        {
        }

        public virtual void PrepareFight()
        {
        }

        public virtual void Update()
        {
            if (_listReport.Count > 1)
            {
                _listReport.Sort(ReportSortHandler);
            }

            if (reciveEvent != null)
            {
                for (int i = 0; i < _listReport.Count; i++)
                {
                    reciveEvent.Invoke(_listReport[i]);
                }

                _listReport.Clear();
            }
        }

        private int ReportSortHandler(FightReport x, FightReport y)
        {
            return x.id.CompareTo(y.id);
        }

        public virtual void SymbolsAdd(string v)
        {
        }

        public virtual void UseSkill(int roleId)
        {
        }

        public virtual void EventCannonClear(int roleId, int type)
        {
        }

        public virtual void EventShipClear(int roleId, int index)
        {
        }

        public virtual void PauseFight()
        {
        }

        public virtual void PlayFight()
        {
        }
    }
}
