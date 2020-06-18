using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamePath : MonoBehaviour {

	public Transform[] waypoints;

	public float heightOffsetOnPlatform=1;
	
	public float dynamicWP=1;
	
	public bool generatePathObject=true;

	public bool showGizmo=true;
	
	private Transform thisT;
	
	private List<PathSection> path = new List<PathSection>();
	
	void Awake()
    {
		thisT=transform;
	}
	
	public void InitPath()
    {
		for(int i=0; i < waypoints.Length; i++)
        {
			Transform wp=waypoints[i];
			
			Platform platform = wp.gameObject.GetComponent<Platform>();
			if (platform != null)
			{
				path.Add(new PathSection(platform));
			}
			else
			{
				path.Add(new PathSection(wp.position));
			}
		}
		
		for(int i=0; i<path.Count; i++)
        {
			PathSection pSec=path[i];
			if(path[i].platform!=null)
            {
				PathSection sec1=path[Mathf.Max(0, i-1)];
				PathSection sec2=path[Mathf.Min(path.Count-1, i+1)];

				pSec.platform.SetPathObject(this, pSec, sec1, sec2);
				pSec.platform.SetWalkable(true);

				if(!pSec.platform.IsNodeGenerated()) 
					pSec.platform.GenerateNode(heightOffsetOnPlatform);
				
				pSec.platform.SearchForNewPath(pSec);
			}
		}

		if(generatePathObject)
        {
			CreateLinePath();
		}
	}

	void CreateLinePath(){
		
		Vector3 offsetPos=new Vector3(0, 0, 0);
		
		for(int i=1; i<waypoints.Length; i++)
        {
			if(path[i].platform==null && path[i-1].platform==null)
            {
				GameObject obj=new GameObject();
				obj.name="path"+i.ToString();
				
				Transform objT=obj.transform;
				objT.parent=thisT;
				
				LineRenderer line=obj.AddComponent<LineRenderer>();
				line.material=(Material)Resources.Load("PathMaterial");
				line.SetWidth(0.3f, 0.3f);
				
				line.SetPosition(0, waypoints[i-1].position+offsetPos);
				line.SetPosition(1, waypoints[i].position+offsetPos);
			}
			//platform to waypoint
			else if(path[i].platform==null && path[i-1].platform!=null)
            {
				GameObject obj=new GameObject();
				obj.name="path"+i.ToString();
				
				Transform objT=obj.transform;
				objT.parent=thisT;
				
				LineRenderer line=obj.AddComponent<LineRenderer>();
				line.material=(Material)Resources.Load("PathMaterial");
				line.SetWidth(0.3f, 0.3f);
				
				List<Vector3> path1=path[i-1].GetSectionPath();
				
				line.SetPosition(0, path1[path1.Count-1]+offsetPos);
				line.SetPosition(1, waypoints[i].position+offsetPos);
			}
			else if(path[i].platform!=null && path[i-1].platform==null)
            {
				GameObject obj=new GameObject();
				obj.name="path"+i.ToString();
				
				Transform objT=obj.transform;
				objT.parent=thisT;
				
				LineRenderer line=obj.AddComponent<LineRenderer>();
				line.material=(Material)Resources.Load("PathMaterial");
				line.SetWidth(0.3f, 0.3f);
				
				List<Vector3> path1=path[i].GetSectionPath();
				
				line.SetPosition(0, waypoints[i-1].position+offsetPos);
				line.SetPosition(1, path1[0]+offsetPos);
			}
			else if(path[i].platform!=null && path[i-1].platform!=null)
            {
				GameObject obj=new GameObject();
				obj.name="path"+i.ToString();
				
				Transform objT=obj.transform;
				objT.parent=thisT;
				
				LineRenderer line=obj.AddComponent<LineRenderer>();
				line.material=(Material)Resources.Load("PathMaterial");
				line.SetWidth(0.3f, 0.3f);
				
				List<Vector3> path1=path[i-1].GetSectionPath();
				List<Vector3> path2=path[i].GetSectionPath();
				
				line.SetPosition(0, path1[path1.Count-1]+offsetPos);
				line.SetPosition(1, path2[0]+offsetPos);
			}
		}
		
		foreach(PathSection ps in path)
        {
			if(ps.platform==null)
				Instantiate((GameObject)Resources.Load("wpNode"), ps.pos+offsetPos, Quaternion.identity);
		}
		
	}
	
	public List<PathSection> GetPath()
    {
		return path;
	}

    public Platform GetPlatform()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            Transform wp = waypoints[i];

            Platform platform = wp.gameObject.GetComponent<Platform>();
			if (platform != null)
			{
				return platform;
			}
        }
        return null;
    }

	void OnDrawGizmos()
    {
		if(showGizmo)
        {
			Gizmos.color = Color.blue;
			if(waypoints!=null && waypoints.Length>0)
            {
				for(int i=1; i<waypoints.Length; i++)
                {
					Gizmos.DrawLine(waypoints[i-1].position, waypoints[i].position);
				}
			}
		}
	}
	
}

public class PathSection
{
	public Platform platform;

	public Vector3 pos;
	
	private List<Vector3> sectionPath=new List<Vector3>();

	private int pathID=0;	
									
	public PathSection(Vector3 p)
    {
		pos=p;
		sectionPath.Add(pos);
	}
	
	public PathSection(Platform p)
    {
		platform=p;
	}
	
	public void SetSectionPath(List<Vector3> L, int id)
    {
        sectionPath =L;
		pathID=id;
	}
	
	public List<Vector3> GetSectionPath()
    {
		return sectionPath;
	}
	
	public int GetPathID()
    {
		return pathID;
	}
}
