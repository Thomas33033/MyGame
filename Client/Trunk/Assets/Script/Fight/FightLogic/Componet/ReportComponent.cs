using UnityEngine;
using System.Collections.Generic;

namespace Fight
{
    public class ReportComponent : BaseComponent
    {
        protected List<FightReport> _listReport;

        public ReportComponent(Role role) : base(role)
        {
            _listReport = new List<FightReport>();
        }

        public void AddReport(FightReport fightReport)
        {
            _listReport.Add(fightReport);
        }

        public void GetReport(ref List<FightReport> listReport)
        {
            listReport.AddRange(_listReport);
            _listReport.Clear();
        }
    }
}