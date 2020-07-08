using UnityEngine;

using FightCommom;

public enum ENodeColor
{
    Empty, //空白

    Block, //占用

    CanBuild,  //可以建造

    CantBuild, //不能建造
}

public class NodeRender : Node
{
    private ENodeColor colorState;
    public int x, y;
    public GameObject viewObj;

    public bool walkable_new;

    public NodeRender() { }

    public NodeRender(Node node, Vector3 position)
    {
        this.Id = node.Id;
        this.pos = position;
        this.walkable = node.walkable;
        this.walkable_new = node.walkable;
        this.x = (int)node.pos.x;
        this.y = (int)node.pos.z;
        this.neighbourNode = node.neighbourNode;
        this.neighbourCost = node.neighbourCost;
        this.parent = node.parent;

        CreateViewObj();
    }

    public void CreateViewObj()
    {
        IndicatorController.Instance.Create((obj) =>
        {
            this.viewObj = obj;
            this.viewObj.gameObject.SetActive(true);
            this.viewObj.name = Id + " " +x +"-"+ y ;
            this.viewObj.transform.position = this.pos;
            this.viewObj.transform.localScale = new Vector3(1, 0.1f, 1);
            this.DefaultColor();
        });
    }

    public void DefaultColor()
    {
        colorState = walkable_new ? ENodeColor.Empty : ENodeColor.Block;
        SetViewColorState(colorState);
    }

    public void SetViewColorState(ENodeColor state)
    {
        if (this.viewObj == null)
            return;

        colorState = state;
        if (!walkable_new)
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
       
        this.viewObj.GetComponent<Renderer>().material.SetColor("_TintColor", color);
    }
}