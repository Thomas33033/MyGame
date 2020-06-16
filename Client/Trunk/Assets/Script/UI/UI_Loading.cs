using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : MonoBehaviour
{
    public static UI_Loading instance;

    public Image proressBar;

    private Action _loadOverEvent;

    private float _startTime;
    private float _animTotalTimes = 0.5f;
    private bool _bPlay = false;
    // Start is called before the first frame update
    void Start()
    {
        proressBar.fillAmount = 0;
        instance = this;
    }

    public void ShowProgress(Action loadOverEvent)
    {
        _startTime = Time.timeSinceLevelLoad;
        _loadOverEvent = loadOverEvent;
        _bPlay = true;
    }

    public void SetProgress()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_bPlay)
        {
            proressBar.fillAmount = Math.Min((Time.timeSinceLevelLoad - _startTime) / _animTotalTimes, 1) ;

            if (proressBar.fillAmount >= 1 && _loadOverEvent != null)
            {
                _loadOverEvent();
            }
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
