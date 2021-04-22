using FightCommom;
using UnityEngine;

public enum ENodeType
{
    Empty, //空白

    Block, //阻挡

    CanBuild,  //可以建造

    CantBuild, //不能建造
}

public class NodeRender : Node
{

    public bool walkable_new;

    public Platform platform;

    public NodeRender() { }

    public NodeRender(Node node, Vector3 position)
    {
        this.Id = node.Id;
        this.pos = position;
        this.walkable = node.walkable;
        this.walkable_new = node.walkable;
        this.neighbourNode = node.neighbourNode;
        this.neighbourCost = node.neighbourCost;
        this.parent = node.parent;
    }

    public void DefaultColor()
    {
        state = walkable_new ? ENodeType.Empty : ENodeType.Block;
        SetViewColorState(state);
    }

    public void SetViewColorState(ENodeType state)
    {
        this.state = state;
    }

}