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
        base.Awake();
        btnNextLevel = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_pause");
        btnRestart = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_spawn");
        btnBack = this.FindObject<Button>(this.gameObject, "UI_Menu_Offset/grid/btn_pause");

        GameTools.AddClickEvent(btnNextLevel.gameObject, OnNextLevelEvent);
        GameTools.AddClickEvent(btnRestart.gameObject, OnRestartEvent);
        GameTools.AddClickEvent(btnBack.gameObject, OnBackEvent);
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
