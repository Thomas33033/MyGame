using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPathComponent : ComponentBase
{
    public bool wpMode = false; //if set to true, use wp, else use path
    public GamePath path;
    public List<Vector3> wp = new List<Vector3>();
    public int wpCounter = 0;


    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }
}
