using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGame : Singleton<MainGame>
{
    private bool b_GamePause = false;

    public bool IsPause()
    {
        return b_GamePause;
    }

    public void GamePause()
    {
        Time.timeScale = 0;
        b_GamePause = true;
    }

    public void GameResume()
    {
        Time.timeScale = 1;
        b_GamePause = false;
    }

    public static float ServerTime
    {
        get{ return Time.time; }
    }
   

    public bool SetGameTime(bool state)
    {
        b_GamePause = state;
      
        return b_GamePause;
    }
    

    public void Init()
    { 

    }

    

}
