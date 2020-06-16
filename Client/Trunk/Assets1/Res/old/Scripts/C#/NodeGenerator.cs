using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class NodeGenerator : MonoBehaviour
{
	[HideInInspector] public Rect area;

	[HideInInspector] public float gridSize=1;

	private float maxSlope=0;         //最大倾斜角度

	public bool connectDiagonalNeighbour=false;
	
	[HideInInspector] public float agentHeight=2;

	static public float _gridSize=1;
	
	static Node[] nodeGraph;

	static NodeGenerator nodeGenerator;

	static private Transform thisT;

	void Awake()
    {
		nodeGenerator = this;
		thisT = transform;
	}
	
	void Start()
    {
		CycleNode();
	}
	
	static public void Init()
    {
		GameObject obj=new GameObject();
		nodeGenerator=obj.AddComponent<NodeGenerator>();
		thisT=obj.transform;
		obj.name="NodeGenerator";
	}
	
	static public void CheckInit()
    {
		if(nodeGenerator==null) Init();
	}
	
	static public bool CheckConnection(Transform p1, Transform p2)
    {
		if(Mathf.Abs(p1.position.y-p2.position.y)>_gridSize/2)
        {
			return false;
		}

		float distX=p1.position.x-p2.position.x;

		float coverageX=p1.localScale.x*10/2+p2.localScale.x*10/2;
		
		if(distX<=coverageX) return true;
		
		float distZ=p1.position.z-p2.position.z;

		float coverageZ=p1.localScale.z*10/2+p2.localScale.z*10/2;
		
		if(distZ<=coverageZ) return true;
		
		return false;
	}

	/// <summary>
    /// 初始化Node节点，并生成每个节点的周围8个点的权重关系
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="heightOffset"></param>
    /// <returns></returns>
	static public Node[] GenerateNode(Platform platform, float heightOffset){
		
		CheckInit();

		float timeStart=Time.realtimeSinceStartup;
		
		Transform platformT=platform.thisT;
		
		float gridSize=BuildManager.GetGridSize();
		
		float scaleX=platform.thisT.localScale.x;
		float scaleZ=platform.thisT.localScale.z;
		
		int countX=(int)(10*scaleX/gridSize);
		int countZ=(int)(10*scaleZ/gridSize);

        platform.row = countZ;
        platform.column = countX;

		float x=-scaleX*10/2/scaleX;
		float z=-scaleZ*10/2/scaleZ;
        //将thisT 定位到平台的坐下角 {0,0}点
        Vector3 point =platformT.TransformPoint(new Vector3(x, 0, z));
		thisT.position=point;
		thisT.rotation=platformT.rotation;
		thisT.position=thisT.TransformPoint(new Vector3(gridSize/2, heightOffset, gridSize/2));

        Node[] nodeGraph=new Node[countZ*countX];
		int counter=0;
        Vector3 tempPosition;
		for(int i=0; i<countZ; i++)
        {
			for(int j=0; j<countX; j++)
            {
                tempPosition = thisT.TransformPoint(new Vector3(gridSize*j, 0, gridSize * i));
                nodeGraph[counter] = new Node(tempPosition, counter);
				counter+=1;
			}
		}

		thisT.position=Vector3.zero;
		thisT.rotation=Quaternion.identity;
		
		float timeUsed=Time.realtimeSinceStartup-timeStart;
		
		counter=0;
		foreach(Node cNode in nodeGraph)
        {
			if(cNode.walkable)
            {
				LayerMask mask = 1<<LayerManager.LayerPlatform();
				Collider[] cols = Physics.OverlapSphere(cNode.pos, gridSize*0.45f, ~mask);
				if(cols.Length>0)
                {
					cNode.walkable=false;
					counter+=1;
				}
			}
		}
		
		float neighbourDistance=0;
		float neighbourRange;
		if(nodeGenerator.connectDiagonalNeighbour) neighbourRange=gridSize*1.5f;
		else neighbourRange=gridSize*1.1f;
		
		timeStart=Time.realtimeSinceStartup;
		
		counter=0;
		foreach(Node currentNode in nodeGraph)
        {
			if(currentNode.walkable)
            {
				List<Node> neighbourNodeList = new List<Node>();
				List<float> neighbourCostList=new List<float>();
				
				Node[] neighbour=new Node[8];
				int id=currentNode.ID;
				
				if(id>countX-1 && id<countX*countZ-countX)
                {
					//print("middle rows");
					if(id!=countX) neighbour[0]=nodeGraph[id-countX-1];
					neighbour[1]=nodeGraph[id-countX];
					neighbour[2]=nodeGraph[id-countX+1];
					neighbour[3]=nodeGraph[id-1];
					neighbour[4]=nodeGraph[id+1];
					neighbour[5]=nodeGraph[id+countX-1];
					neighbour[6]=nodeGraph[id+countX];
					if(id!=countX*countZ-countX-1)neighbour[7]=nodeGraph[id+countX+1];
				}
				else if(id<=countX-1)
                {
					//print("first row");
					if(id!=0) neighbour[0]=nodeGraph[id-1];
					if(nodeGraph.Length>id+1) neighbour[1]=nodeGraph[id+1];
					if(countZ>0)
                    {
						if(nodeGraph.Length>id+countX-1)	neighbour[2]=nodeGraph[id+countX-1];
						if(nodeGraph.Length>id+countX)	neighbour[3]=nodeGraph[id+countX];
						if(nodeGraph.Length>id+countX+1)	neighbour[4]=nodeGraph[id+countX+1];
					}
				}
				else if(id>=countX*countZ-countX)
                {
					//print("last row");
					neighbour[0]=nodeGraph[id-1];
					if(id!=countX*countZ-1) neighbour[1]=nodeGraph[id+1];
					neighbour[2]=nodeGraph[id-countX-1];
					neighbour[3]=nodeGraph[id-countX];
					neighbour[4]=nodeGraph[id-countX+1];
				}
				
				foreach(Node node in neighbour)
                {
					if(node!=null && node.walkable)
                    {
						neighbourDistance=GetHorizontalDistance(currentNode.pos, node.pos);
						if(neighbourDistance<neighbourRange){
							if(!Physics.Linecast(currentNode.pos, node.pos))
                            {
								neighbourNodeList.Add(node);
								neighbourCostList.Add(neighbourDistance);
							}
						}
					}
				}

				currentNode.SetNeighbour(neighbourNodeList, neighbourCostList);
			}
			
			counter+=1;
		}

		return nodeGraph;
	}

	void GenerateNode()
    {
		agentHeight=Mathf.Max(agentHeight, gridSize);

		area.x=Mathf.Floor(area.x/gridSize)*gridSize;
		area.y=Mathf.Floor(area.y/gridSize)*gridSize;
		
		area.width=Mathf.Floor(area.width/gridSize)*gridSize;
		area.height=Mathf.Floor(area.height/gridSize)*gridSize;

		nodeGraph=new Node[(int)((area.width-area.x)/gridSize*(area.height-area.y)/gridSize)];

		float timeStart=Time.realtimeSinceStartup;

		int counter=0;
		float heightOffset=agentHeight/2;
		for(float j=area.y; j<area.height; j+=gridSize)
        {
			for(float i=area.x; i<area.width; i+=gridSize)
            {
				RaycastHit hit1;
				if(Physics.Raycast(new Vector3(i, 500, j), Vector3.down, out hit1))
                {
					nodeGraph[counter]=new Node(new Vector3(i, hit1.point.y+heightOffset, j), counter);
				}
				else
                {
					nodeGraph[counter]=new Node(new Vector3(i, 0, j), counter);
					nodeGraph[counter].walkable=false;
				}
				counter+=1;
			}
		}

		float timeUsed=Time.realtimeSinceStartup-timeStart;

		counter=0;
		RaycastHit hit2;
		foreach(Node cNode in nodeGraph)
        {
			if(cNode.walkable)
            {
				if(Physics.SphereCast(cNode.pos+new Vector3(0, heightOffset+heightOffset*0.1f, 0), gridSize*0.45f, Vector3.down, out hit2, heightOffset))
                {
					cNode.walkable=false;
					counter+=1;
				}
			}
		}

		float neighbourDistance=0;
		float neighbourRange;
		if(connectDiagonalNeighbour)	neighbourRange=gridSize*1.5f;
		else neighbourRange=gridSize*1.1f;
		
		timeStart=Time.realtimeSinceStartup;
		
		int rowLength=(int)Mathf.Floor((area.width-area.x)/gridSize);
		int columnLength=(int)Mathf.Floor((area.height-area.y)/gridSize);

		counter=0;

		foreach(Node currentNode in nodeGraph)
        {
			if(currentNode.walkable){
			
				List<Node> neighbourNodeList=new List<Node>();
				List<float> neighbourCostList=new List<float>();
				
				Node[] neighbour=new Node[8];
				int id=currentNode.ID;
				
				if(id>rowLength-1 && id<rowLength*columnLength-rowLength)
                {
					//print("middle rows");
					if(id!=rowLength) neighbour[0]=nodeGraph[id-rowLength-1];
					neighbour[1]=nodeGraph[id-rowLength];
					neighbour[2]=nodeGraph[id-rowLength+1];
					neighbour[3]=nodeGraph[id-1];
					neighbour[4]=nodeGraph[id+1];
					neighbour[5]=nodeGraph[id+rowLength-1];
					neighbour[6]=nodeGraph[id+rowLength];
					if(id!=rowLength*columnLength-rowLength-1)neighbour[7]=nodeGraph[id+rowLength+1];
				}
				else if(id<=rowLength-1){
					//print("first row");
					if(id!=0) neighbour[0]=nodeGraph[id-1];
					neighbour[1]=nodeGraph[id+1];
					neighbour[2]=nodeGraph[id+rowLength-1];
					neighbour[3]=nodeGraph[id+rowLength];
					neighbour[4]=nodeGraph[id+rowLength+1];
				}
				else if(id>=rowLength*columnLength-rowLength)
                {
					//print("last row");
					neighbour[0]=nodeGraph[id-1];
					if(id!=rowLength*columnLength-1) neighbour[1]=nodeGraph[id+1];
					neighbour[2]=nodeGraph[id-rowLength-1];
					neighbour[3]=nodeGraph[id-rowLength];
					neighbour[4]=nodeGraph[id-rowLength+1];
				}
				
				foreach(Node node in neighbour)
                {
					if(node!=null && node.walkable)
                    {
						neighbourDistance=GetHorizontalDistance(currentNode.pos, node.pos);
						if(neighbourDistance<neighbourRange)
                        {
							if(!Physics.Linecast(currentNode.pos, node.pos))
                            {
								if(Mathf.Abs(GetSlope(currentNode.pos, node.pos))<=maxSlope){
									neighbourNodeList.Add(node);
									neighbourCostList.Add(neighbourDistance);
								}
							}
						}
					}
				}
				currentNode.SetNeighbour(neighbourNodeList, neighbourCostList);
			}
			counter+=1;
		}
	}


	static float GetHorizontalDistance(Vector3 p1, Vector3 p2)
    {
		p1.y=0;
		p2.y=0;
		return Vector3.Distance(p1, p2);
	}


	float GetSlope(Vector3 p1, Vector3 p2)
    {
		var h1=p1.y;
		var h2=p2.y;
		
		float distH=GetHorizontalDistance(p1, p2);
		float slope=(h1-h2)/distH;
		
		return Mathf.Atan(slope)*Mathf.Rad2Deg;
	}

	Node GetNearestWalkableNode(Vector3 point)
    {
		float dist=Mathf.Infinity;
		float currentNearest=Mathf.Infinity;
		Node nearestNode=null;

		foreach(Node node in nodeGraph)
        {
			if(node.walkable)
            {
				dist=Vector3.Distance(point, node.pos);
				if(dist<currentNearest)
                {
					currentNearest=dist;
					nearestNode=node;
				}
			}
		}
		
		return nearestNode;
	}

	Node GetNearestUnwalkableNode(Vector3 point)
    {
		float dist=Mathf.Infinity;
		float currentNearest=Mathf.Infinity;
		Node nearestNode=null;
		foreach(Node node in nodeGraph)
        {
			if(!node.walkable)
            {
				dist=Vector3.Distance(point, node.pos);
				if(dist<currentNearest)
                {
					currentNearest=dist;
					nearestNode=node;
				}
			}
		}
		return nearestNode;
	}

	void BlockNode(Vector3 pos){
		Node node=GetNearestWalkableNode(pos);
		pos.y=node.pos.y;
		if(Vector3.Distance(pos, node.pos)<NodeGenerator._gridSize/2)
			node.walkable=false;
	}

	void UnblockNode(Vector3 pos){
		Node node=GetNearestUnwalkableNode(pos);
		if(node!=null){
			pos.y=node.pos.y;
			if(Vector3.Distance(pos, node.pos)<NodeGenerator._gridSize/2)
				node.walkable=true;
		}
	}

	[HideInInspector] public bool showGizmo=true;
	private Node currentNode;

	IEnumerator CycleNode()
    {
		int counter=0;
		while(true)
        {
			yield return new WaitForSeconds(0.15f);
			currentNode=nodeGraph[counter];
			counter+=1;
			if(counter==nodeGraph.Length) counter=0;
		}
	}

	void OnDrawGizmos()
    {
		if(showGizmo)
        {
			if(nodeGraph!=null)
            {
				Gizmos.color = Color.white;
				foreach(Node node in nodeGraph)
                {
					if(node!=null && node.walkable) Gizmos.DrawSphere (node.pos, 0.2f);
					else if(node!=null && !node.walkable) Gizmos.DrawSphere (node.pos, 0.5f);
				}
			}
			
			if(currentNode!=null)
            {
				Gizmos.color = Color.red;
				foreach(Node neighbour in currentNode.neighbourNode){
					Gizmos.DrawSphere (neighbour.pos, 0.2f);
				}
			}
		}
	}

}

public enum _ListState{Unassigned, Open, Close};

public class Node
{
	public int ID;
	public Vector3 pos;
	public Node[] neighbourNode;
	public float[] neighbourCost;
	public Node parent;
    private bool mWalkable = true;
	public bool walkable
    {
        get { return mWalkable; }
        set {
            mWalkable = value;
            DefaultColor();
        }
    }
	public float scoreG;
	public float scoreH;
	public float scoreF;
	public _ListState listState =_ListState.Unassigned;
	public float tempScoreG=0;
    public GameObject viewObj;
   
    public Node(){}
	
	public Node(Vector3 position, int id)
    {
		pos=position;
		ID=id;
    }
	
	public void SetNeighbour(List<Node> arrNeighbour, List<float> arrCost)
    {
		neighbourNode = arrNeighbour.ToArray();
		neighbourCost = arrCost.ToArray();
	}
	
	public void ProcessNeighbour(Node node)
    {
		ProcessNeighbour(node.pos);
	}
	
	public void ProcessNeighbour(Vector3 pos)
    {
		for(int i=0; i<neighbourNode.Length; i++)
        {
			if(neighbourNode[i].listState==_ListState.Unassigned)
            {
				neighbourNode[i].scoreG=scoreG+neighbourCost[i];
				neighbourNode[i].scoreH=Vector3.Distance(neighbourNode[i].pos, pos);
				neighbourNode[i].UpdateScoreF();
				neighbourNode[i].parent=this;
			}
			else if(neighbourNode[i].listState==_ListState.Open)
            {
				tempScoreG=scoreG+neighbourCost[i];
				if(neighbourNode[i].scoreG>tempScoreG)
                {
					neighbourNode[i].parent=this;
					neighbourNode[i].scoreG=tempScoreG;
					neighbourNode[i].UpdateScoreF();
				}
			}
		}
	}
	
	void UpdateScoreF()
    {
		scoreF=scoreG+scoreH;
	}

    public void CreateViewObj()
    {
        IndicatorController.Instance.Create((obj)=>{
            this.viewObj = obj;
            this.viewObj.gameObject.SetActive(true);
            this.viewObj.name = ID + "";
            this.viewObj.transform.position = pos;
            this.viewObj.transform.localScale = new Vector3(1, 0.1f, 1);
            this.DefaultColor();
        }); 
    }

    public void DefaultColor()
    {
       this.SetViewColor(mWalkable ? Color.white*0.2f : Color.red);
    }

    public void SetViewColor(Color color)
    {
        if (this.viewObj == null)
            return;
        if (mWalkable == false)
        {
            this.viewObj.GetComponent<Renderer>().material.SetColor("_TintColor", Color.red);
        }
        else
        {
            this.viewObj.GetComponent<Renderer>().material.SetColor("_TintColor", color);
        }
       
    }

}

[System.Serializable]
public class PlaneConnection : MonoBehaviour{
	public Transform plane1;
	public Transform plane2;
	public bool isConnected=false;
	public bool isAligned=false;
	public Vector3[] overlapPoint;
}