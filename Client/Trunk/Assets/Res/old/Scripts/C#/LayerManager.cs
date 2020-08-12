using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LayerManager : MonoBehaviour {
	
	public static bool initiated=false;

    public int layerCreep;
    public int layerCreepF;
    public int layerTower;
    public int layerPlatform;
    public int layerOverlay;

	public int layerTerrain;

	public static int layerBuilding;

	public static LayerManager layerManager;


    private void OnInit()
    {
        //layerCreep = LayerMask.NameToLayer("layerCreep");
        //layerCreepF = LayerMask.NameToLayer("layerCreepF");
        //layerTower = LayerMask.NameToLayer("layerTower");
        layerPlatform = LayerMask.NameToLayer("layerPlatform");
		layerTerrain = LayerMask.NameToLayer("Terrain");
		layerBuilding = LayerMask.NameToLayer("Building");
		//layerOverlay = LayerMask.NameToLayer("layerOverlay");
	}

    public LayerMask GetMask(int layer)
    {
        return 1 << layer;
    }


    void Awake(){
        OnInit();
        layerManager =this;
		
		#if UNITY_EDITOR
			GameControl gameControl=gameObject.GetComponent<GameControl>();
			if(gameControl!=null){
				gameControl.layerManager=this;
			}
		#endif
	}

	
	
	public static void Init(){
		if(layerManager==null){
			GameObject obj=new GameObject();
			obj.name="LayerManager";
			
			layerManager=obj.AddComponent<LayerManager>();
			
			//Debug.Log("init   "+layerManager);
		}
	}
	
	public static int LayerCreep()
    {
		return layerManager.layerCreep;
		//return layerCreep;
	}
	
	public static int LayerCreepF(){
		return layerManager.layerCreepF;
		//return layerCreepF;
	}
	
	public static int LayerTower(){
		return layerManager.layerTower;
		//return layerTower;
	}
	
	public static int LayerPlatform(){
		return layerManager.layerPlatform;
		//return layerPlatform;
	}
	
	public static int LayerOverlay(){
		return layerManager.layerOverlay;
		//return layerPlatform;
	}

	public static int LayerTerrain()
	{
		return layerManager.layerTerrain;
	}


}
