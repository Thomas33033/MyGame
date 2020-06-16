using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public delegate void SetPathCallback(List<Vector3> wp);

public class PathFinder {

	static private List<SearchQueue> queue=new List<SearchQueue>();

	static bool searching=false;

	static PathFinder pathFinder;
	
	public bool pathSmoothing=false;

	public int ScanNodeLimitPerFrame=350;
	
	static public bool IsPathSmoothingOn()
    {
		return pathFinder.pathSmoothing;
	}
	
	void Awake()
    {
		pathFinder=this;
	}
	
	void Start()
    {

	}
	
	static public void CheckInit(){
		if (pathFinder == null)
			pathFinder = new PathFinder();
	}
	
	
	static public void CallMeBack(SetPathCallback callBackFunc){

	}

    void Update()
    {


    }

    static public Node GetNearestNode(Vector3 point, Node[] graph){
		float dist=Mathf.Infinity;
		float currentNearest=Mathf.Infinity;
		Node nearestNode=null;
		foreach(Node node in graph){
			if(node.walkable){
				dist=Vector3.Distance(point, node.pos);
				if(dist<currentNearest){
					currentNearest=dist;
					nearestNode=node;
				}
			}
		}
		return nearestNode;
	}
	
	
	
	static public void GetPath(Vector3 startP, Vector3 endP, Node[] graph, SetPathCallback callBackFunc){
		Node startNode=GetNearestNode(startP, graph);
		Node endNode=GetNearestNode(endP, graph);
		
		GetPath(startNode, endNode, null, graph, callBackFunc, false);
	}
	
	static public void GetPath(Vector3 startP, Vector3 endP, Vector3 blockP, Node[] graph, SetPathCallback callBackFunc){
		Node startNode=GetNearestNode(startP, graph);
		Node endNode=GetNearestNode(endP, graph);
		Node blockNode=GetNearestNode(blockP, graph);
		
		GetPath(startNode, endNode, blockNode, graph, callBackFunc, false);
	}
	
	static public void GetPath(Node startN, Node endN, Node[] graph, SetPathCallback callBackFunc){
		GetPath(startN, endN, null, graph, callBackFunc, false);
	}
	
	static public void GetPath(Node startN, Node endN, Node[] graph, SetPathCallback callBackFunc, bool urgent){
		GetPath(startN, endN, null, graph, callBackFunc, urgent);
	}
	
	static public void GetPath(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc, bool urgent){
		CheckInit();
		
		if(!searching){
			Search(startN, endN, blockN, graph, callBackFunc);
		}
		else
        {
			SearchQueue q=new SearchQueue(startN, endN, blockN, graph, callBackFunc);
			if(urgent) queue.Insert(0, q);
			else queue.Add(q);
		}
	}
	
	static private void Search(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc){
		Main.Instance.StartCoroutine(pathFinder._Search(startN, endN, blockN, graph, callBackFunc));
	}
	
	IEnumerator _Search(Node startN, Node endN, Node blockN, Node[] graph, SetPathCallback callBackFunc){
		
		if(blockN!=null)
        {
			blockN.walkable=false;
		}

		float timeStart=Time.realtimeSinceStartup;

		searching=true;

		bool pathFound=true;
		
		int searchCounter=0;
        	
		int loopCounter=0;		

		List<Node> closeList=new List<Node>();

		Node[] openList=new Node[graph.Length];
		
		List<int> openListRemoved=new List<int>();

		int openListCounter=0;

		Node currentNode=startN;
		
		float currentLowestF=Mathf.Infinity;
		int id=0;	
		int i=0;		

		while(true)
        {
			if(currentNode==endN) break;

			closeList.Add(currentNode);

			currentNode.listState=_ListState.Close;
			
			currentNode.ProcessNeighbour(endN);
			
			foreach(Node neighbour in currentNode.neighbourNode)
            {
				if(neighbour.listState==_ListState.Unassigned && neighbour.walkable)
                {
					neighbour.listState=_ListState.Open;
					if(openListRemoved.Count>0)
                    {
						openList[openListRemoved[0]]=neighbour;
						openListRemoved.RemoveAt(0);
					}
					else
                    {
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}

			currentNode=null;
			currentLowestF=Mathf.Infinity;
			id=0;
			for(i=0; i<openListCounter; i++){
				if(openList[i]!=null){
					if(openList[i].scoreF<currentLowestF){
						currentLowestF=openList[i].scoreF;
						currentNode=openList[i];
						id=i;
					}
				}
			}

			if(currentNode==null) {
				pathFound=false;
				break;
			}

			openList[id]=null;
			openListRemoved.Add(id);
			searchCounter+=1;
			loopCounter+=1;

			if(loopCounter>ScanNodeLimitPerFrame){
				loopCounter=0;	
				yield return null;
			}
		}

		List<Vector3> p=new List<Vector3>();
			
		if(pathFound)
        {
			while(currentNode!=null)
            {
				p.Add(currentNode.pos);
				currentNode=currentNode.parent;
			}
			
			p=InvertArray(p);
			if(pathSmoothing) {
				p=LOSPathSmoothingBackward(p);
				p=LOSPathSmoothingForward(p);
			}
		}

		if(blockN!=null) blockN.walkable=true; 
		
		callBackFunc(p);

		searching=false;
		
		ResetGraph(graph);
	
	}
	
	
	static public List<Vector3> ForceSearch(Node startN, Node endN, Node blockN, Node[] graph){
		
		if(blockN!=null){
			blockN.walkable=false;
		}
		
		bool pathFound=true;
		
		int searchCounter=0;
		
		List<Node> closeList=new List<Node>();
		Node[] openList=new Node[graph.Length];
		
		List<int> openListRemoved=new List<int>();
		int openListCounter=0;

		Node currentNode=startN;
		
		float currentLowestF=Mathf.Infinity;
		int id=0;	
		int i=0;	
		
		while(true){
		
			if(currentNode==endN) break;
			
			closeList.Add(currentNode);
			currentNode.listState=_ListState.Close;
			
			currentNode.ProcessNeighbour(endN);
			
			foreach(Node neighbour in currentNode.neighbourNode)
            {
				if(neighbour.listState==_ListState.Unassigned && neighbour.walkable)
                {
					neighbour.listState=_ListState.Open;
					if(openListRemoved.Count>0)
                    {
						openList[openListRemoved[0]]=neighbour;
						openListRemoved.RemoveAt(0);
					}
					else
                    {
						openList[openListCounter]=neighbour;
						openListCounter+=1;
					}
				}
			}
			
			currentNode=null;
			
			currentLowestF=Mathf.Infinity;
			id=0;
			for(i=0; i<openListCounter; i++)
            {
				if(openList[i]!=null){
					if(openList[i].scoreF<currentLowestF)
                    {
						currentLowestF=openList[i].scoreF;
						currentNode=openList[i];
						id=i;
					}
				}
			}
			
			if(currentNode==null)
            {
				pathFound=false;
				break;
			}
			
			openList[id]=null;
			openListRemoved.Add(id);

			searchCounter+=1;
			
		}

		List<Vector3> p=new List<Vector3>();
			
		if(pathFound)
        {
			while(currentNode!=null)
            {
				p.Add(currentNode.pos);
				currentNode=currentNode.parent;
			}
			p=InvertArray(p);
		}
		
		if(blockN!=null)
        {
			blockN.walkable=true; 
		}
		
		ResetGraph(graph);
		
		return p;

	}
	
	static public List<Vector3> SmoothPath(List<Vector3> p)
    {
		if(pathFinder.pathSmoothing) {
			p=pathFinder.LOSPathSmoothingBackward(p);
			p=pathFinder.LOSPathSmoothingForward(p);
		}
		
		return p;
	}
	
	static private List<Vector3> InvertArray(List<Vector3> p){
		List<Vector3> pInverted=new List<Vector3>();
		for(int i=0; i<p.Count; i++){
			pInverted.Add(p[p.Count-(i+1)]);
		}
		return pInverted;
	}
	
	private List<Vector3> LOSPathSmoothingForward(List<Vector3> p){
		float gridSize=BuildManager.GetGridSize();
		int num=0;
		float allowance=gridSize*0.4f;
		while (num+2<p.Count){
			bool increase=false;
			Vector3 p1=p[num];
			Vector3 p2=p[num+2];
			RaycastHit hit;
			Vector3 dir=p2-p1;
			
			if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1))){
				if(p1.y==p2.y) p.RemoveAt(num+1);
				else increase=true;
			}
			else {
				increase=true;
			}
			
			if(increase) num+=1;

		}
		return p;
	}

	private List<Vector3> LOSPathSmoothingBackward(List<Vector3> p){
		float gridSize=BuildManager.GetGridSize();
		int num=p.Count-1;
		float allowance=gridSize*0.4f;
		while (num>1){
			bool decrease=false;
			Vector3 p1=p[num];
			Vector3 p2=p[num-2];
			RaycastHit hit;
			Vector3 dir=p2-p1;
			
			if(!Physics.SphereCast(p1, allowance, dir, out hit, Vector3.Distance(p2, p1))){
				if(p1.y==p2.y) p.RemoveAt(num-1);
				else decrease=true;
			}
			else {
				decrease=true;
			}
			
			num-=1;
			if(decrease) num-=1;

		}
		return p;
	}
	
	static public void ResetGraph(Node[] nodeGraph){
		foreach(Node node in nodeGraph){
			node.listState=_ListState.Unassigned;
			node.parent=null;
		}
	}
	
}


class SearchQueue{
	public Node startNode;
	public Node endNode;
	public Node blockNode;
	public Node[] graph;
	public SetPathCallback callBackFunc;
	
	public SearchQueue(Node n1, Node n2, Node n3, Node[] g, SetPathCallback func){
		startNode=n1;
		endNode=n2;
		blockNode=n3;
		graph=g;
		callBackFunc=func;
	}
}
