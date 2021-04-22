using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FightCommom;
using Fight;
using UnityEngine.SocialPlatforms;

public class NodeGenerator : MonoBehaviour
{
	[HideInInspector] public Rect area;

	[HideInInspector] public float gridSize=1;

	private float maxSlope=0;         //最大倾斜角度

	public bool connectDiagonalNeighbour=false;
	
	[HideInInspector] public float agentHeight=2;

	static public float _gridSize=1;

	static NodeGenerator nodeGenerator;

	static public Transform thisT;

    private static LayerMask ignorMask;

	void Awake()
    {
		nodeGenerator = this;
		thisT = transform;
	}
	
	void Start()
    {
        //ignorMask = ~(1 << LayerManager.LayerPlatform() | 1 << LayerManager.LayerTerrain());
		ignorMask = LayerManager.GetBlockMask();
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
	/// 初始化Node节点，并生成每个节点周围，8个邻居的权重关系
	/// </summary>
	/// <param name="platform"></param>
	/// <param name="heightOffset"></param>
	/// <returns></returns>
	static public void GenerateNode(Platform platform, float heightOffset){
		
		CheckInit();

		Transform platformT = platform.thisT;
		
		int gridSize = (int)BuildManager.GetGridSize();
		
		float scaleX = platform.thisT.localScale.x;
		float scaleZ = platform.thisT.localScale.z;
		
		int countX=(int)(10*scaleX/gridSize);
		int countZ=(int)(10*scaleZ/gridSize);

		IntVector2 platformSize = new IntVector2(countX, countZ);
		platform.row = countX;
        platform.column = countZ;
		platform.gridSize = gridSize;

		float x = -scaleX*10/2/scaleX ;

		float z = -scaleZ*10/2/scaleZ;

		//将thisT 定位到平台的左下角 {0,0}点
		Vector3 point = platformT.TransformPoint(new Vector3(x, 0, z));
		thisT.position = point;
		thisT.rotation = platformT.rotation;
		thisT.position = thisT.TransformPoint(new Vector3(gridSize/2f, heightOffset, gridSize/2f));

		int counter = 0;
		
		Vector3 nodePos;

		var nodes = new Node[countX, countZ];
		
		float radius = gridSize * 0.5f;
		for (int i = 0; i < countX; i++)
		{
			for (int j = 0; j < countZ; j++)
			{
				//主要考虑到平台倾斜，从而使用矩阵转化
				nodePos = thisT.TransformPoint(new Vector3(gridSize * i, 0, gridSize * j));
				Node node = new Node(new Vector3(i, 0, j), counter++);
				node.worldPos = nodePos;
				if (Physics.CheckSphere(nodePos, radius, ignorMask))
				{
					node.walkable = false;
					node.SetWeight(int.MaxValue);
					node.state = ENodeType.Block;
				}
				nodes[i, j] = node;
			}
		}

		SetNeighbours(nodes, platformSize, gridSize);

		//List<NodeData> jsonObjs = new List<NodeData>();
		//for (int i = 0; i < nodeGraph.Length; i++)
		//{
		//	var nodeData = new NodeData(nodeGraph[i]);
		//	jsonObjs.Add(nodeData);
		//}

		//string json = SimpleJson.SimpleJson.SerializeObject(jsonObjs);
		//StaticData.SaveData("GridData.json", json);
		platform.nodeGraph = nodes;
	}

	public static void SetNeighbours(Node[,] nodes, IntVector2 platformSize, int gridSize)
	{
        float neighbourDistance = 0;
        float neighbourRange;
        if (nodeGenerator.connectDiagonalNeighbour)
            neighbourRange = gridSize * 1.5f;
        else
            neighbourRange = gridSize * 1.1f;

        Node currentNode;
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                currentNode = nodes[i, j];

                if (currentNode.walkable)
                {
                    List<Node> neighbours = GetAllNeighbours(currentNode.gridPos, platformSize, nodes);

                    List<float> neighbourCostList = new List<float>();
                    List<Node> neighbourNodeList = new List<Node>();

                    foreach (Node node in neighbours)
                    {
                        if (node != null && node.walkable)
                        {
                            neighbourDistance = GetHorizontalDistance(currentNode.gridPos, node.gridPos);
                            if (neighbourDistance < neighbourRange)
                            {
                                //if(!Physics.Linecast(currentNode.pos, node.pos))
                                {
                                    neighbourNodeList.Add(node);
                                    neighbourCostList.Add(neighbourDistance);
                                }
                            }
                        }
                    }
                    currentNode.SetNeighbour(neighbourNodeList, neighbourCostList);
                }
            }
        }
    }


	//void GenerateNode()
	//   {
	//	agentHeight=Mathf.Max(agentHeight, gridSize);

	//	area.x=Mathf.Floor(area.x/gridSize)*gridSize;
	//	area.y=Mathf.Floor(area.y/gridSize)*gridSize;

	//	area.width=Mathf.Floor(area.width/gridSize)*gridSize;
	//	area.height=Mathf.Floor(area.height/gridSize)*gridSize;

	//	nodeGraph=new Node[(int)((area.width-area.x)/gridSize*(area.height-area.y)/gridSize)];

	//	float timeStart=Time.realtimeSinceStartup;

	//	int counter=0;
	//	float heightOffset=agentHeight/2;
	//	for(int j=(int)area.y; j<area.height; j+= (int)gridSize)
	//       {
	//		for(int i = (int)area.x; i<area.width; i+= (int)gridSize)
	//           {
	//			RaycastHit hit1;
	//			if(Physics.Raycast(new Vector3(i, 500, j), Vector3.down, out hit1))
	//               {
	//				nodeGraph[counter]=new Node(new Vector3(i, hit1.point.y+heightOffset, j), counter);
	//			}
	//			else
	//               {
	//				nodeGraph[counter]=new Node(new Vector3(i, 0, j), counter);
	//				nodeGraph[counter].walkable=false;
	//			}
	//			counter+=1;
	//		}
	//	}

	//	float timeUsed=Time.realtimeSinceStartup-timeStart;

	//	counter=0;
	//	RaycastHit hit2;
	//	foreach(Node cNode in nodeGraph)
	//       {
	//		if(cNode.walkable)
	//           {
	//			if(Physics.SphereCast(cNode.pos+new Vector3(0, heightOffset+heightOffset*0.1f, 0), gridSize*0.45f, Vector3.down, out hit2, heightOffset))
	//               {
	//				cNode.walkable=false;
	//				counter+=1;
	//			}
	//		}
	//	}

	//	float neighbourDistance=0;
	//	float neighbourRange;
	//	if(connectDiagonalNeighbour)	neighbourRange=gridSize*1.5f;
	//	else neighbourRange=gridSize*1.1f;

	//	timeStart=Time.realtimeSinceStartup;

	//	int rowLength=(int)Mathf.Floor((area.width-area.x)/gridSize);
	//	int columnLength=(int)Mathf.Floor((area.height-area.y)/gridSize);

	//	counter=0;

	//	foreach(Node currentNode in nodeGraph)
	//       {
	//		if(currentNode.walkable){


	//		//	GetAllNeighbours();

	//			List<Node> neighbourNodeList=new List<Node>();
	//			List<float> neighbourCostList=new List<float>();

	//			Node[] neighbour=new Node[8];
	//			int id=currentNode.Id;

	//			if(id>rowLength-1 && id<rowLength*columnLength-rowLength)
	//               {
	//				//print("middle rows");
	//				if(id!=rowLength) neighbour[0]=nodeGraph[id-rowLength-1];
	//				neighbour[1]=nodeGraph[id-rowLength];
	//				neighbour[2]=nodeGraph[id-rowLength+1];
	//				neighbour[3]=nodeGraph[id-1];
	//				neighbour[4]=nodeGraph[id+1];
	//				neighbour[5]=nodeGraph[id+rowLength-1];
	//				neighbour[6]=nodeGraph[id+rowLength];
	//				if(id!=rowLength*columnLength-rowLength-1)neighbour[7]=nodeGraph[id+rowLength+1];
	//			}
	//			else if(id<=rowLength-1){
	//				//print("first row");
	//				if(id!=0) neighbour[0]=nodeGraph[id-1];
	//				neighbour[1]=nodeGraph[id+1];
	//				neighbour[2]=nodeGraph[id+rowLength-1];
	//				neighbour[3]=nodeGraph[id+rowLength];
	//				neighbour[4]=nodeGraph[id+rowLength+1];
	//			}
	//			else if(id>=rowLength*columnLength-rowLength)
	//               {
	//				//print("last row");
	//				neighbour[0]=nodeGraph[id-1];
	//				if(id!=rowLength*columnLength-1) neighbour[1]=nodeGraph[id+1];
	//				neighbour[2]=nodeGraph[id-rowLength-1];
	//				neighbour[3]=nodeGraph[id-rowLength];
	//				neighbour[4]=nodeGraph[id-rowLength+1];
	//			}

	//			foreach(Node node in neighbour)
	//               {
	//				if(node!=null && node.walkable)
	//                   {
	//					neighbourDistance=GetHorizontalDistance(currentNode.gridPos, node.gridPos);
	//					if(neighbourDistance<neighbourRange)
	//                       {
	//						if(!Physics.Linecast(currentNode.pos, node.pos))
	//                           {
	//							if(Mathf.Abs(GetSlope(currentNode.pos, node.pos))<=maxSlope){
	//								neighbourNodeList.Add(node);
	//								neighbourCostList.Add(neighbourDistance);
	//							}
	//						}
	//					}
	//				}
	//			}
	//			currentNode.SetNeighbour(neighbourNodeList, neighbourCostList);
	//		}
	//		counter+=1;
	//	}
	//}

	public static bool IsValid(int x, int y, IntVector2 gridSize, Node[,] grid)
    {
        return x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y && grid[x, y].GetWeight() != int.MaxValue;
    }

    private static List<Node> GetAllNeighbours(IntVector2 v, IntVector2 gridSize, Node[,] grid)
    {
        List<Node> res = new List<Node>();
        int x = v.x;
        int y = v.y;
        bool up = IsValid(x, y - 1, gridSize, grid),
            down = IsValid(x, y + 1, gridSize, grid),
            left = IsValid(x - 1, y, gridSize, grid),
            right = IsValid(x + 1, y, gridSize, grid);

        if (left)
        {
            res.Add(grid[x - 1, y]);

            //left up
            if (up && IsValid(x - 1, y - 1, gridSize, grid))
            {
                res.Add(grid[x - 1, y - 1]);
            }
        }

        if (up)
        {
            res.Add(grid[x, y - 1]);

            //up right
            if (right && IsValid(x + 1, y - 1, gridSize, grid))
            {
                res.Add(grid[x + 1, y - 1]);
            }
        }

        if (right)
        {
            res.Add(grid[x + 1, y]);

            //right down
            if (down && IsValid(x + 1, y + 1, gridSize, grid))
            {
                res.Add(grid[x + 1, y + 1]);
            }
        }

        if (down)
        {
            res.Add(grid[x, y + 1]);

            //down left
            if (left && IsValid(x - 1, y + 1, gridSize, grid))
            {
                res.Add(grid[x - 1, y + 1]);
            }
        }
        return res;
    }

    static float GetHorizontalDistance(IntVector2 p1, IntVector2 p2)
    {
		return IntVector2.Distance(p1, p2);
	}


    //   float GetSlope(Vector3 p1, Vector3 p2)
    //   {
    //       var h1 = p1.y;
    //       var h2 = p2.y;

    //	float distH = 0;//GetHorizontalDistance(p1, p2);
    //       float slope = (h1 - h2) / distH;

    //       return Mathf.Atan(slope)*Mathf.Rad2Deg;
    //}


	//void BlockNode(Vector3 pos){
	//	Node node=GetNearestWalkableNode(pos);
	//	pos.y=node.pos.y;
	//	if(Vector3.Distance(pos, node.pos)<NodeGenerator._gridSize/2)
	//		node.walkable=false;
	//}

	//void UnblockNode(Vector3 pos){
	//	Node node=GetNearestUnwalkableNode(pos);
	//	if(node!=null){
	//		pos.y=node.pos.y;
	//		if(Vector3.Distance(pos, node.pos)<NodeGenerator._gridSize/2)
	//			node.walkable=true;
	//	}
	//}

	[HideInInspector] public bool showGizmo=true;
	private Node currentNode;

}



