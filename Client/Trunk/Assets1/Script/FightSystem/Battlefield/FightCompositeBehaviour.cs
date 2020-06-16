using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Fight
{
    /// <summary>
    /// 战场行为
    /// </summary>
    public class FightCompositeBehaviour
    {
        protected FightData _fightData;

        public string owerId { get => _fightData.selfBattleData.userData.userID; }

        protected List<FightReport> _listReport;

        protected int _fightType;

        public delegate void ReciveFightReportHandler(FightReport report);

        public event ReciveFightReportHandler reciveEvent;

        static public FightCompositeBehaviour instance;
        public virtual bool isFight => false;

        private void Awake()
        {
            _listReport = new List<FightReport>();

            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
            reciveEvent = null;
        }

        public virtual float GetTime()
        {
            return 0f;
        }

        //public void ReciveEventInvoke(FightReport fightReport)
        //{
        //    this.reciveEvent.Invoke(fightReport);
        //}

        public virtual void InitFight(int fightType, FightData fightData)
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

        public virtual void UseSkill(string roleId)
        {
        }

        public virtual void EventCannonClear(string roleId, int type)
        {
        }

        public virtual void EventShipClear(string roleId, int index)
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
