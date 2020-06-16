using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoSingleton<InitOnStart>
    {
        public List<int> dataList = new List<int>();

        public Button Btn_Delete;
        public Button Btn_Insert;
        public Button Btn_Refresh;
        private LoopScrollRect mLoopList;
        void Start()
        {
            for (int i = 1; i < 15; i++)
            {
                dataList.Add(i*10);
            }

            mLoopList = GetComponent<LoopScrollRect>();
            mLoopList.totalCount = dataList.Count;
            mLoopList.SetScrollCellChangeEvent(this.OnRefreshData);
            
            mLoopList.RefillCells();

            GameTools.AddClickEvent(Btn_Delete.gameObject, OnDeleteBtnClick);
            GameTools.AddClickEvent(Btn_Insert.gameObject, OnInsertBtnClick);
            GameTools.AddClickEvent(Btn_Refresh.gameObject, OnRefreshBtnClick);
        }


        private void OnDeleteBtnClick()
        {
            dataList.RemoveAt(dataList.Count - 1);
            mLoopList.SetTotalCount(dataList.Count);
            mLoopList.RefreshCells();
            
        }

        private void OnInsertBtnClick()
        {
            dataList.Insert(4,51);
            mLoopList.SetTotalCount(dataList.Count);
            mLoopList.RefreshCells();
        }

        private void OnRefreshBtnClick()
        {
            //GetComponent<LoopScrollRect>()；
        }

        private void OnRefreshData(Transform trans, int index)
        {
            trans.GetComponent<ScrollIndexCallback1>().ScrollCellIndex(dataList[index]);
        }
    }
}