//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class ObjectPoolManager : MonoBehaviour {
	
////	[SerializeField] 
//    static private List<Pool> poolList=new List<Pool>();
//    static private bool cumulative=true;
	
//    static private ObjectPoolManager opm;
//    static private Transform thisT;
	
//    static int currentIDCount=0;
	
//    static public void SetCumulativeFlag(bool flag)
//    {
//        cumulative=flag;
//    }
	
//    static public bool GetCumulativeFlag()
//    {
//        return cumulative;
//    }
	
//    static public void New(GameObject obj, int num)
//    {
//        New(obj, num, cumulative);
//    }
	
//    static public void New(AssetsType type,string resName)
//    {
       
//    }
	
//    static public void New(Transform obj, int num, bool flag)
//    {
//        New(obj.gameObject, num);
//    }
	
//    static public void New(GameObject obj, int num, bool flag)
//    {
//        int objExist=CheckIfObjectExist(obj);
		
//        if(objExist>=0)
//        {
//            if(flag) poolList[objExist].PrePopulate(num);
//            else poolList[objExist].MatchPopulation(num);
			
//            #if UNITY_EDITOR
//                poolList[objExist].HideInHierarchy(thisT);
//            #endif
//        }
//        else
//        {
//            currentIDCount+=1;
			
//            poolList.Add(new Pool());

//            Pool newPool=new Pool(obj, num, currentIDCount-1);
//            poolList[newPool.ID]=newPool;
			
//            #if UNITY_EDITOR
//                poolList[newPool.ID].HideInHierarchy(thisT);
//            #endif
//        }
//    }
	
//    //check to see if the new object exist
//    static private int CheckIfObjectExist(GameObject obj){
//        int match=0;
//        foreach(Pool pool in poolList){
//            if(obj==pool.GetPrefab()){
//                return match;
//            }
//            match+=1;
//        }
//        return -1;
//    }
	
//    //check to see if an object is tagged with an ID
//    static private int CheckIfObjectIsTagged(GameObject obj){
//        ObjectID objectID=obj.GetComponent<ObjectID>();
//        if(objectID==null) return -1;
//        else return objectID.GetID();
//    }


//    static public GameObject Spawn(GameObject obj){
//        return Spawn(obj, Vector3.zero, Quaternion.identity);
//    }
	
//    static public GameObject Spawn(Transform obj){
//        return Spawn(obj.gameObject, Vector3.zero, Quaternion.identity);
//    }
	
//    static public GameObject Spawn(Transform obj, Vector3 pos, Quaternion rot){
//        return Spawn(obj.gameObject, pos, rot);
//    }

//    static public GameObject Spawn(GameObject obj, Vector3 pos, Quaternion rot){
//        GameObject spawnObj;

//        int ID=CheckIfObjectExist(obj);
		
//        if(ID==-1){
//            Debug.Log("Object "+obj+" doesnt exsit in ObjectPoolManager List.");
//            return (GameObject)Instantiate(obj, pos, rot);
//        }
//        else{
//            spawnObj=poolList[ID].Spawn(obj, pos, rot);
//        }
		
//        #if UNITY_EDITOR
//            spawnObj.transform.parent=null;
//            //if(opm.hideObjInHierarchy) spawnObj.transform.parent=null;
//        #endif
		
//        return spawnObj;
//    }
	
	
//    static public void Unspawn(Transform obj){
//        Unspawn(obj.gameObject);
//    }
	
//    static public void Unspawn(GameObject obj){
		
//        int ID=CheckIfObjectIsTagged(obj);
		
//        #if UNITY_EDITOR
//            obj.transform.parent=thisT;
//            //if(opm.hideObjInHierarchy) obj.transform.parent=thisT;
//        #endif
		
//        if(ID==-1){
//            Debug.Log("Object doesnt exsit in ObjectPoolManager List");
//            obj.SetActiveRecursively(false);
//        }
//        else{
//            poolList[ID].Unspawn(obj);
//        }
		
//    }
	
//    //Delayed unspawn
//    static public void Unspawn(Transform obj, float duration){
//        opm.StartCoroutine(opm.UnspawnTimed(obj, duration));
//    }
	
//    static public void Unspawn(GameObject obj, float duration){
//        opm.StartCoroutine(opm.UnspawnTimed(obj, duration));
//    }
	
//    private IEnumerator UnspawnTimed(Transform obj, float duration){
//        yield return new WaitForSeconds(duration);
//        Unspawn(obj);
//    }
	
//    private IEnumerator UnspawnTimed(GameObject obj, float duration){
//        yield return new WaitForSeconds(duration);
//        Unspawn(obj);
//    }
	
//    static public void Init(){
//        ClearAll();
		
//        currentIDCount=0;
		
//        GameObject obj=new GameObject();
//        obj.name="ObjectPoolManager";
		
//        opm=obj.AddComponent<ObjectPoolManager>();
		
//        thisT=obj.transform;
//    }
	
//    static public void ClearAll(){
//        foreach(Pool pool in poolList){
//            pool.UnspawnAll();
//        }
//        poolList=new List<Pool>();
//    }
	
//    static public List<GameObject> GetList(GameObject obj){
//        //int ID=CheckIfObjectIsTagged(obj);
//        int ID=CheckIfObjectExist(obj);
		
//        if(ID>=0) return poolList[ID].GetFullList();
//        else {
//            Debug.Log("Object doesnt exsit in ObjectPoolManager List.");
			
//            return new List<GameObject>();
//        }
//    }
	
//}



//[System.Serializable]
//public class Pool {
	
//    public int ID=-1;
	
//    private GameObject prefab;
//    private int totalObjCount;

//    private List<GameObject> available=new List<GameObject>();
//    private List<GameObject> allObject=new List<GameObject>();
	
//    private bool setActiveRecursively=false;

//    public Pool(){}
	
//    public Pool(GameObject obj, int num, int id){
//        prefab=obj;
//        ID=id;
//        if(prefab.transform.childCount>0) setActiveRecursively=true;
//        PrePopulate(num);
//    }
	
//    public void MatchPopulation(int num)
//    {
//        PrePopulate(num-totalObjCount);
//    }
	
//    public void PrePopulate(int num)
//    {
//        for(int i=0; i<num; i++)
//        {
//            GameObject obj = (GameObject)GameObject.Instantiate(prefab);
//            obj.AddComponent<ObjectID>().SetID(ID);
//            available.Add(obj);
//            allObject.Add(obj);
//            totalObjCount+=1;
//            if(!setActiveRecursively) obj.SetActive(false);
//            else obj.SetActive(false);
//        }
//    }
	
//    public GameObject Spawn(GameObject obj, Vector3 pos, Quaternion rot){
//        GameObject spawnObj;
		
//        if(available.Count>0){
//            spawnObj=available[0];
//            available.RemoveAt(0);
			
//            if(!setActiveRecursively) spawnObj.active=true;
//            else spawnObj.SetActiveRecursively(true);
			
//            Transform tempT=spawnObj.transform;
			
//            tempT.position=pos;
//            tempT.rotation=rot;
//        }
//        else{
//            spawnObj=(GameObject)GameObject.Instantiate(prefab, pos, rot);
//            spawnObj.AddComponent<ObjectID>().SetID(ID);
//            totalObjCount+=1;
//        }
		
//        return spawnObj;
//    }
	
//    public void Unspawn(GameObject obj){
//        available.Add(obj);
		
//        if(!setActiveRecursively) obj.active=false;
//        else obj.SetActiveRecursively(false);
//    }
	
//    public void UnspawnAll(){
//        foreach(GameObject obj in allObject){
//            if(obj!=null) GameObject.Destroy(obj);
//        }
//    }
	
//    public void HideInHierarchy(Transform t){
//        foreach(GameObject obj in allObject){
//            obj.transform.parent=t;
//        }
//    }
	
//    public GameObject GetPrefab(){
//        return prefab;
//    }
	
//    public int GetTotalCount(){
//        return totalObjCount;
//    }
	
//    public List<GameObject> GetFullList(){
//        Debug.Log("getting list, list length= "+allObject.Count);
//        return allObject;
//    }
//}


//[System.Serializable]
//public class ObjectID : MonoBehaviour{
//    public int ID=-1;
	
//    public void SetID(int id){
//        ID=id;
//    }
	
//    public int GetID(){
//        return ID;
//    }
//}