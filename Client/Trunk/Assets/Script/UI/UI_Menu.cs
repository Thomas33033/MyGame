using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Menu : UIPanelBase {

    public Button btnNextLevel;
    public Button btnRestart;
    public Button btnBack;

    void Awake()
    {
        btnNextLevel = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_pause");
        btnRestart = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_spawn");
        btnBack = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_pause");

        UITools.AddClickEvent(btnNextLevel.gameObject, OnNextLevelEvent);
        UITools.AddClickEvent(btnRestart.gameObject, OnRestartEvent);
        UITools.AddClickEvent(btnBack.gameObject, OnBackEvent);
	}

    void OnNextLevelEvent()
    { 
    
    }

    void OnRestartEvent()
    {

    }

    void OnBackEvent()
    { 
    
    }
}
