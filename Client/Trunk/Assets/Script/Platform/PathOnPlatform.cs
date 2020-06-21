using UnityEngine;
using UnityEditor;
using FightCommom;
using System.Collections.Generic;

/// <summary>
/// /路径平台
/// </summary>
public class PathOnPlatform
{
    public GamePath path;

    public PathSection thisWP;

    public PathSection prevNeighbouringWP;

    public PathSection nextNeighbouringWP;

    public Node startN;

    public Node endN;

    public List<Vector3> currentPath = new List<Vector3>();

    public int pathID = 0;

    public List<Vector3> altPath = new List<Vector3>();

    public PathOnPlatform(GamePath p, PathSection pSec, PathSection prev, PathSection next)
    {
        path = p;
        thisWP = pSec;
        prevNeighbouringWP = prev;
        nextNeighbouringWP = next;
    }

    public void InitNode(Node[] nodeGraph)
    {
        Vector3 prevPoint;
        if (prevNeighbouringWP.platform != null)
        {
            prevPoint = prevNeighbouringWP.platform.thisT.position;
        }
        else prevPoint = prevNeighbouringWP.pos;

        startN = PathFinder.GetNearestNode(prevPoint, nodeGraph);

        Vector3 nextPoint;

        if (nextNeighbouringWP.platform != null)
        {
            nextPoint = nextNeighbouringWP.platform.thisT.position;
        }
        else nextPoint = nextNeighbouringWP.pos;

        endN = PathFinder.GetNearestNode(nextPoint, nodeGraph);
        if (endN != null)
        {
            Debug.DrawLine(endN.pos, endN.pos + new Vector3(0, 2, 0), Color.red, 50);
            Debug.DrawLine(startN.pos, startN.pos + new Vector3(0, 2, 0), Color.blue, 50);
            Debug.DrawLine(nextNeighbouringWP.pos, nextNeighbouringWP.pos + new Vector3(0, 2, 0), Color.green, 50);
        }
        
    }

    public void SetPath(List<Vector3> wp, int id)
    {
        currentPath = wp;
        pathID = id;
        thisWP.SetSectionPath(wp, pathID);
    }

    public void SmoothPath()
    {
        currentPath = PathFinder.SmoothPath(currentPath);
    }
}