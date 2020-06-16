using UnityEngine;
using UnityEditor;

public class FightSence : Singleton<FightSence>
{
    public GameObject rootUI;

    public float GetTime()
    {
        return Time.realtimeSinceStartup;
    }
}
