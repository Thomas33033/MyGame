using UnityEngine;
using UnityEditor;

using FightCommom;

public enum ENodeColor
{
    Empty, //空白

    Block, //占用

    CanBuild,  //可以建造

    CantBuild, //不能建造
}

public class NodeRender
{
    public Node node;

    public bool walkable => node.walkable;

    private ENodeColor colorState;
    public GameObject viewObj;

    public NodeRender(Node node)
    {
        this.node = node;
        CreateViewObj();
    }

    public void CreateViewObj()
    {

        IndicatorController.Instance.Create((obj) =>
        {
            this.viewObj = obj;
            this.viewObj.gameObject.SetActive(true);
            this.viewObj.name = node.ID + " " +node.x +"-"+node.y ;
            this.viewObj.transform.position = node.pos;
            this.viewObj.transform.localScale = new Vector3(1, 0.1f, 1);
            this.DefaultColor();
        });
    }

    public void DefaultColor()
    {
        colorState = node.walkable ? ENodeColor.Empty : ENodeColor.Block;
        SetViewColorState(colorState);
    }

    public void SetViewColorState(ENodeColor state)
    {
        if (this.viewObj == null)
            return;

        colorState = state;
        if (!node.walkable)
        {
            colorState = ENodeColor.Block;
        }
    
        RefreshColor();
    }

    public void RefreshColor()
    {
        Color color = Color.white * 0.2f;
        switch (colorState)
        {
            case ENodeColor.Block:
                color = Color.red;
                break;
            case ENodeColor.Empty:
                color = Color.white * 0.2f;
                break;
            case ENodeColor.CanBuild:
                color = Color.green;
                break;
            case ENodeColor.CantBuild:
                color = Color.yellow;
                break;
        }
       // Debug.Log(this.node.ID + "  " + colorState);
        this.viewObj.GetComponent<Renderer>().material.SetColor("_TintColor", color);
    }
}