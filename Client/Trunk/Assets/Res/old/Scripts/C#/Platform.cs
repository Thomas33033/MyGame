using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fight;
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
	
	public List<Vector3> currentPath=new List<Vector3>();

	public int pathID=0;
	
	public List<Vector3> altPath=new List<Vector3>();
	
	public PathOnPlatform(GamePath p, PathSection pSec, PathSection prev, PathSection next)
    {
       
        path =p;
		thisWP=pSec;
		prevNeighbouringWP=prev;
		nextNeighbouringWP=next;
	}
	
	public void InitNode(Node[] nodeGraph)
    {
		Vector3 prevPoint;
		if(prevNeighbouringWP.platform!=null)
        {
			prevPoint=prevNeighbouringWP.platform.thisT.position;
		}
		else prevPoint =prevNeighbouringWP.pos;

		startN=PathFinder.GetNearestNode(prevPoint, nodeGraph);
		
		Vector3 nextPoint;

		if(nextNeighbouringWP.platform!=null)
        {
			nextPoint=nextNeighbouringWP.platform.thisT.position;
		}
		else nextPoint=nextNeighbouringWP.pos;

		endN=PathFinder.GetNearestNode(nextPoint, nodeGraph);
		
		Debug.DrawLine(endN.pos, endN.pos+new Vector3(0, 2, 0), Color.red, 50);
		Debug.DrawLine(startN.pos, startN.pos+new Vector3(0, 2, 0), Color.blue, 50);
		Debug.DrawLine(nextNeighbouringWP.pos, nextNeighbouringWP.pos+new Vector3(0, 2, 0), Color.green, 50);
	}
	
	public void SetPath(List<Vector3> wp, int id)
    {
		currentPath=wp;
		pathID=id;
		thisWP.SetSectionPath(wp, pathID);
	}
	
	public void SmoothPath()
    {
		currentPath=PathFinder.SmoothPath(currentPath);
	}
}

public class Platform : MonoBehaviour {
	
	public _TowerType[] buildableType=new _TowerType[1];

	public int[] specialBuildableID;
	
	private bool walkable;

	private Node[] nodeGraph;

    private Dictionary<int, Node> nodeGraphMap;

	private bool graphGenerated=false;

	public bool IsNodeGenerated()
    {
		return graphGenerated;
	}

	private List<PathOnPlatform> pathObjects=new List<PathOnPlatform>();

	private Node nextBuildNode;   //距离建筑最近的格子
    

    [HideInInspector] public GameObject thisObj;
	[HideInInspector] public Transform thisT;

    public int row;
    public int column;
    
    //表现
    public bool GizmoShowNodes = true;
    public bool GizmoShowPath = true;

    public int ID;

    void Awake()
    {
        nodeGraphMap = new Dictionary<int, Node>();

        ID = Entity.GetUniqueId();

        thisObj =gameObject;

		thisT=transform;
		
		thisObj.layer=LayerManager.LayerPlatform();
		
		if(specialBuildableID!=null && specialBuildableID.Length>0)
        {
			for(int i=0; i<specialBuildableID.Length; i++)
            {
				specialBuildableID[i]=Mathf.Max(0, specialBuildableID[i]);
			}
		}
	}
	
	void Start()
    {
		
	}
	
	public void GenerateNode(float heightOffset)
    {
		nodeGraph = NodeGenerator.GenerateNode(this, heightOffset);
        for (int i = 0; i < nodeGraph.Length; i++ )
        {
            nodeGraphMap.Add(nodeGraph[i].ID, nodeGraph[i]);
        }
        
        graphGenerated =true;
	}
	
	private List<PathSection> queue= new List<PathSection>();
	
	public void SearchForNewPath(PathSection PS){
		queue.Add(PS);
		foreach(PathOnPlatform pathPlatform in pathObjects)
        {
			if(pathPlatform.thisWP==PS)
            {
                pathPlatform.InitNode(nodeGraph);
				PathFinder.GetPath(pathPlatform.startN, pathPlatform.endN, nodeGraph, this.SetPath);
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
		
		nextBuildNode=PathFinder.GetNearestNode(pos, nodeGraph);

        //如果建筑需要的格子包含阻挡，则返回false
        if (RefreshBulidGrid(nextBuildNode.ID, costGrid))
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

            pathObj.altPath = PathFinder.ForceSearch(pathObj.startN, pathObj.endN, nextBuildNode, nodeGraph);

            if (pathObj.altPath.Count == 0)
            {
                Debug.LogError("pathObj.altPath.Count: " + pathObj.altPath.Count);
                blocked = true;
            }

            if (blocked) break;
        }

        return blocked;
		
	}

    public int GetPostion()
    {
        if (this.nextBuildNode != null)
        {
            return this.nextBuildNode.ID;
        }
        else {
            return 0;
        }
    }


	public void Build(Vector3 point, FightHeroData buildData, List<int> costNodeIDs)
    {
        if (walkable)
        {

            if (nextBuildNode!=null && Vector3.Distance(nextBuildNode.pos, point) < BuildManager.GetGridSize()/2)
            {
				nextBuildNode.walkable=false;
                for (int i = 0; i < costNodeIDs.Count; i++)
                {
                    nodeGraphMap[costNodeIDs[i]].walkable = false;
                    nodeGraphMap[costNodeIDs[i]].DefaultColor();
                }

                buildData.PlatformId = this.ID;
                buildData.Position = nextBuildNode.ID;

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
                buildData.Position = nextBuildNode.ID;

                foreach (PathOnPlatform pathObj in pathObjects)
                {
					PathFinder.GetPath(pathObj.startN, pathObj.endN, nodeGraph, this.SetPath);
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
			PathFinder.GetPath(pathObj.startN, pathObj.endN, nodeGraph, this.SetPath);
		}
	}

	public void SetWalkable(bool flag)
    {
		walkable=flag;
	}
	
	public bool IsWalkable(){
		return walkable;
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
                if (node.viewObj == null)
                {
                    node.CreateViewObj();
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
        for (int i = 0; i < nodeGraph.Length; i++)
        {
            nodeGraph[i].DefaultColor();
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
                nodeGraph[list[i]].SetViewColor(hasBlock == true ? Color.red : Color.blue);
                costGrid.Add(list[i]);
            }
        }


        return hasBlock;
    }
	
	

}
