using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FightCommom;
using Fight;


public class Platform : MonoBehaviour {
	
	public _TowerType[] buildableType=new _TowerType[1];
	
	private bool walkable;

	private Node[] nodeGraph;

    private Dictionary<int, NodeRender> nodeGraphMap;

	private bool graphGenerated=false;

	public bool IsNodeGenerated()
    {
		return graphGenerated;
	}

	private List<PathOnPlatform> pathObjects=new List<PathOnPlatform>();

	private Node nearestNode;   //距离建筑最近的格子
    

    [HideInInspector] public GameObject thisObj;
	[HideInInspector] public Transform thisT;

    public int row;
    public int column;
    
    //表现
    public bool GizmoShowNodes = true;
    public bool GizmoShowPath = true;

    public int ID;

    private List<PathSection> queue = new List<PathSection>();

    void Awake()
    {
        nodeGraphMap = new Dictionary<int, NodeRender>();

        ID = Entity.GetUniqueId();

        thisObj =gameObject;

		thisT=transform;
		
		//thisObj.layer=LayerManager.LayerPlatform();
    
	}
	
	public void GenerateNode(float heightOffset)
    {
		nodeGraph = NodeGenerator.GenerateNode(this, heightOffset);
        for (int i = 0; i < nodeGraph.Length; i++ )
        {
            nodeGraphMap.Add(nodeGraph[i].ID, new NodeRender(nodeGraph[i]));
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
	
	
	public void SetPath(List<Vector3> wp)
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


    public bool CheckForBlock(Vector3 pos, List<int> costGrid)
    {
		float gridSize=BuildManager.GetGridSize();
		bool blocked=false;
		
		nearestNode=PathFinder.GetNearestNode(pos, nodeGraph);

        //如果建筑需要的格子包含阻挡，则返回false
        if (RefreshBulidGrid(nearestNode.ID, costGrid))
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


	public void Build(Vector3 point, FightHeroData buildData, List<int> costNodeIDs)
    {
        if (walkable)
        {

            if (nearestNode!=null && Vector3.Distance(nearestNode.pos, point) < BuildManager.GetGridSize()/2)
            {
				nearestNode.walkable=false;
                for (int i = 0; i < costNodeIDs.Count; i++)
                {
                    nodeGraphMap[costNodeIDs[i]].node.walkable = false;
                    nodeGraphMap[costNodeIDs[i]].DefaultColor();
                }

                buildData.PlatformId = this.ID;
                buildData.Position = nearestNode.pos.ToString();

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
                buildData.Position = nearestNode.pos.ToString();

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
        return this.nodeGraphMap[nodeId].node.pos;
    }


    public Node[] GetNodeGraph(){
		return nodeGraph;
	}

    private bool InitViewObj = false;
	void Update()
    {

        if (InitViewObj == false && nodeGraph != null && nodeGraph.Length > 0)
        {
            foreach (Node node in nodeGraph)
            {
                if(nodeGraphMap[node.ID].viewObj == null)
                {
                    nodeGraphMap[node.ID].CreateViewObj();
                }
            }
            InitViewObj = true;
        }

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
						Gizmos.DrawLine(pathObj.altPath[i], pathObj.altPath[i-1]);
					}
				}
			}
		}
		
	}

    public void CheckColumn(ref bool hasBlock, List<int> list, int _rowIndex, int _columnIndex)
    {
        int id = _rowIndex * this.column + _columnIndex;
        
        if (_columnIndex < 0 || _columnIndex >= this.column)
        {
            hasBlock = true;
        }
        else if (_rowIndex < 0 || _rowIndex >= this.row)
        {
            hasBlock = true;
        }
        else
        {
            if (id >= 0 && id < nodeGraph.Length)
            {
                if (!nodeGraph[id].walkable)
                {
                    hasBlock = true;
                }
                list.Add(id);
            }
        }
    }

    public void ResetDefaultRes()
    {
        foreach(var v in nodeGraphMap)
        {
            v.Value.DefaultColor();
        }
    }

    public bool RefreshBulidGrid(int index, List<int> costGrid)
    {
        ResetDefaultRes();

        List<int> list = new List<int>();
        int c_row = index / this.column;
        int c_column = index % this.column;
        bool hasBlock = false;
        for (int i = 1; i <= 1; i++)
        {
            CheckColumn(ref hasBlock, list, c_row + i, c_column - i);
            CheckColumn(ref hasBlock, list, c_row + i, c_column);
            CheckColumn(ref hasBlock, list, c_row + i, c_column + i);

            CheckColumn(ref hasBlock, list, c_row , c_column - i);
            CheckColumn(ref hasBlock, list, c_row , c_column);
            CheckColumn(ref hasBlock, list, c_row , c_column + i);

            CheckColumn(ref hasBlock, list, c_row - i, c_column - i);
            CheckColumn(ref hasBlock, list, c_row - i, c_column);
            CheckColumn(ref hasBlock, list, c_row - i, c_column + i);
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] >= 0 && list[i] < nodeGraph.Length)
            {
                nodeGraphMap[nodeGraph[list[i]].ID].SetViewColorState(
                    hasBlock == true ? ENodeColor.CantBuild : ENodeColor.CanBuild);
                costGrid.Add(list[i]);
            }
        }

        return hasBlock;
    }

}
