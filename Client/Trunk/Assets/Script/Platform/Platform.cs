using UnityEngine;
using System.Collections.Generic;
using FightCommom;
using Fight;
using System.Runtime.CompilerServices;
using System.Linq;

public class Platform : MonoBehaviour {

    public _TowerType[] buildableType = new _TowerType[1];
    //表现
    public bool GizmoShowNodes = true;

    public bool GizmoShowPath = true;

    public int gridSize;

    public int row;

    public int column;

    public int ID;

    private bool walkable;

    public IntVector2 platformSize;

    public Node[,] nodeGraph;
    public Dictionary<int,Node> nodeMap;

    private bool graphGenerated=false;


    public CreateGridMesh meshGrid;

    public GameObject cube;

    private bool updateGride = true;

    public bool IsNodeGenerated()
    {
		return graphGenerated;
	}

	private List<PathOnPlatform> pathObjects = new List<PathOnPlatform>();

	private Node nearestNode;   //距离建筑最近的格子
    
    [HideInInspector] public GameObject thisObj;

	[HideInInspector] public Transform thisT;

    private List<PathSection> queue = new List<PathSection>();

    public void SetNodeState(int id, ENodeType type)
    {
        this.nodeMap[id].SetNodeState(type);
    }

    public void SetNodeState(IntVector2 pos, ENodeType type)
    {
        this.nodeGraph[pos.x, pos.y].SetNodeState(type);
    }

    public void SetNodeState(int x,int y, ENodeType type)
    {
        this.nodeGraph[x,y].SetNodeState(type);
    }

    public Node[,] GetNodeGraph()
    {
        return nodeGraph;
    }

    public Node GetNodeRender(int x, int y)
    {
        if (x < this.column && y < this.row)
        {
            return this.nodeGraph[x,y];
        }
        return null;
    }


    public void SetNodeWalkState(Node node, bool walkable)
    {
        node.walkable = walkable;
    }
        
    void Awake()
    {
        ID = Entity.GetUniqueId();

        thisObj = gameObject;

		thisT = transform;
	}
	
	public void GenerateNode(float heightOffset)
    {
        NodeGenerator.GenerateNode(this, heightOffset);
        nodeMap = new Dictionary<int, Node>();
        platformSize = new IntVector2(column, row);

        for (int i = 0; i < this.nodeGraph.GetLength(0); i++)
        {
            for (int j = 0; j < this.nodeGraph.GetLength(0); j++)
            {
                nodeMap.Add(nodeGraph[i, j].Id, nodeGraph[i, j]);
            }
        }

        graphGenerated =true;
	}


	public void SearchForNewPath(PathSection PS){
		queue.Add(PS);
		foreach(PathOnPlatform pathPlatform in pathObjects)
        {
			if(pathPlatform.thisWP==PS)
            {
                pathPlatform.InitNode(nodeGraph);
                this.SetPath(PathFinder.GetPath(pathPlatform.startN, pathPlatform.endN, nodeGraph));
			}
		}
	}
	
	public void SetPathObject(GamePath p, PathSection pSec, PathSection prev, PathSection next)
    {
		PathOnPlatform path=new PathOnPlatform(p, pSec, prev, next);
		
		pathObjects.Add(path);
	}
	
	
	public void SetPath(List<Node> wp)
    {
		int pathID=queue[0].GetPathID();
		int rand=pathID;
		while(rand==pathID)
        {
			rand=Random.Range(-999999, 999999);
		}
		pathID=rand;
		
		foreach(PathOnPlatform path in pathObjects)
        {
			if(path.thisWP==queue[0])
            {
				path.SetPath(wp, pathID);
			}
		}
		
		queue.RemoveAt(0);
	}


    public bool CheckForBlock(Node node, List<int> costGrid, int nodeSize)
    {
		float gridSize = BuildManager.GetGridSize();

		bool blocked = false;

        nearestNode = node;

        Vector3 pos = node.GetWorldPosition();

        //如果建筑需要的格子包含阻挡，则返回false
        if (CheckBulidGrid(nearestNode, costGrid, nodeSize))
        {
            return true;
        }

        foreach (PathOnPlatform pathObj in pathObjects)
        {
            if (Vector3.Distance(pos, pathObj.startN.pos) < gridSize / 2) return true;
            if (Vector3.Distance(pos, pathObj.endN.pos) < gridSize / 2) return true;

            if (!PathFinder.IsPathSmoothingOn())
            {
                bool InCurrentPath = false;

                foreach (Vector3 pathPoint in pathObj.currentPath)
                {
                    float dist = Vector3.Distance(pos, pathPoint);
                    if (dist < gridSize / 2)
                    {
                        InCurrentPath = true;
                        break;
                    }
                }

                if (InCurrentPath)
                {
                    blocked = true;
                }
            }
            pathObj.altPath = PathFinder.ForceSearch(pathObj.startN, pathObj.endN, nearestNode, nodeGraph);

            if (pathObj.altPath.Count == 0)
            {
                Debug.LogError("pathObj.altPath.Count: " + pathObj.altPath.Count);
                blocked = true;
            }

            if (blocked) break;
        }

        return blocked;
		
	}

    public Node GetPostion()
    {
        return this.nearestNode;
    }


	public void Build(Vector3 point, FightRoleData buildData, List<IntVector2> costNodeIDs)
    {
        if (walkable)
        {

            if (nearestNode!=null && Vector3.Distance(nearestNode.pos, point) < BuildManager.GetGridSize()/2)
            {
				nearestNode.walkable=false;
                for (int i = 0; i < costNodeIDs.Count; i++)
                {
                    nodeGraph[costNodeIDs[i].x, costNodeIDs[i].y].walkable = false;
                }

                buildData.PlatformId = this.ID;

                foreach (PathOnPlatform pathObj in pathObjects)
                {
					int rand=pathObj.pathID;
                    rand = TimeSchedule.GetUniqueId();
                    pathObj.SetPath(pathObj.altPath, rand);
					pathObj.SmoothPath();
				}
                    
            }
			else
            {
				Node node=PathFinder.GetNearestNode(point, nodeGraph);
				node.walkable=false;
                buildData.PlatformId = this.ID;

                foreach (PathOnPlatform pathObj in pathObjects)
                {
                    this.SetPath(PathFinder.GetPath(pathObj.startN, pathObj.endN, nodeGraph));
				}
            }
			
		}
	}
	
	
	public void UnBuild(Node node)
    {
		node.walkable=true;
		foreach(PathOnPlatform pathObj in pathObjects)
        {
			queue.Add(pathObj.thisWP);
            this.SetPath(PathFinder.GetPath(pathObj.startN, pathObj.endN, nodeGraph));
		}
	}

	public void SetWalkable(bool flag)
    {
		walkable=flag;
	}
	
	public bool IsWalkable(){
		return walkable;
	}

    public Vector3 GetWorldPosition(int nodeId)
    {
        return this.nodeMap[nodeId].GetWorldPosition();
    }

    public Vector3 GetWorldPosition(IntVector2 pos)
    {
        return this.nodeGraph[pos.x,pos.y].GetWorldPosition();
    }

    private bool InitViewObj = false;

    public LayerMask ignorMask;

    void Update()
    {

        if (GizmoShowPath){
			foreach(PathOnPlatform pathObj in pathObjects)
            {
				if(pathObj.currentPath.Count>0)
                {
					Gizmos.color =new Color(1.0f, 0f, 0f, 1.0f);
					foreach(Vector3 p in pathObj.currentPath)
                    {
						Gizmos.color-=new Color(1.0f/pathObj.currentPath.Count, 0, 0, 0);
						Gizmos.color+=new Color(0, 1.0f/pathObj.currentPath.Count, 0, 0);
						Gizmos.DrawSphere (p, 0.5f);
					}
				}
				
				if(pathObj.altPath.Count>0)
                {
					for(int i=1; i<pathObj.altPath.Count; i++)
                    {
						Gizmos.color=Color.red;
						Gizmos.DrawLine(pathObj.altPath[i].pos, pathObj.altPath[i-1].pos);
					}
				}
			}
		}
    }


    private float fNodeDiameter = 1;
    private float fDistanceBetweenNodes = 0.5f;

    public void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));
        if (nodeGraph != null)//If the grid is not empty
        {
            fNodeDiameter = (int)BuildManager.GetGridSize();
            for (int i = 0; i < nodeGraph.GetLength(0); i++)
            {
                for (int j = 0; j < nodeGraph.GetLength(1); j++)
                {
                    Gizmos.color = nodeGraph[i, j].walkable ? Color.red * 0.5f : Color.yellow;
                    Gizmos.DrawCube(nodeGraph[i, j].GetWorldPosition(), new Vector3(1, 0, 1) * (fNodeDiameter - fDistanceBetweenNodes));//Draw the node at the position of the node.
                    // Gizmos.DrawLine(n.GetWorldPosition(), n.GetWorldPosition() + new Vector3(n.getFlowFieldVector().x, 0, n.getFlowFieldVector().y).normalized * 2);
                    // Gizmos.color = Color.black;
                }
            }
        }
    }


    public bool CheckGridIsValid(int x, int y)
    {
        if (x < 0 || x >= this.column)
        {
            return false;
        }
        else if (y < 0 || y >= this.row)
        {
            return false;
        }
        return true;
    }

    public bool CheckGridIsValid(IntVector2 pos)
    {
        return CheckGridIsValid(pos.x, pos.y);
    }
    
    public void Check(ref bool hasBlock, List<int> list, int x, int y)
    {
        if (CheckGridIsValid(x,y))
        {
            hasBlock = true;
        }
        else
        {
            if (!nodeGraph[x,y].walkable)
            {
                hasBlock = true;
            }
            list.Add(nodeGraph[x, y].Id);
        }
    }

    public bool CheckBulidGrid(Node node, List<int> costGrid,int nodeSize)
    {
        costGrid.Clear();

        int startX = node.gridPos.x;

        int startY = node.gridPos.y;

        bool hasBlock = false;

        int range = nodeSize / 2;

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (NodeGenerator.IsValid(startX + x, startY + y, platformSize, this.nodeGraph))
                {
                    costGrid.Add(nodeGraph[startX + x, startY + y].Id);
                }
                else
                {
                    hasBlock = true;
                }
                
            }
        }

        return hasBlock;
    }

    public void GridStateChange()
    {
        this.updateGride = true;
    }

    public Node PositionToNode(Vector3 position)
    {
        Vector3 pos = NodeGenerator.thisT.InverseTransformPoint(position);
        int x = Mathf.FloorToInt(pos.x / this.gridSize);
        int y = Mathf.FloorToInt(pos.z / this.gridSize);
        if (x < this.column && y < this.row)
        {
            return nodeGraph[x, y];
        }
        else
        {
            return nodeGraph[0, 0];
        }
       
    }
}

