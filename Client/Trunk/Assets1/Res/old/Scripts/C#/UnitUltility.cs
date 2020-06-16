using UnityEngine;
using System.Collections;

public class UnitUtility : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	static public void DestroyColliderRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.gameObject.GetComponent<Collider>()!=null)  Destroy(child.gameObject.GetComponent<Collider>());
			DestroyColliderRecursively(child);
		}
	}
	
	static public void DisableColliderRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.gameObject.GetComponent<Collider>()!=null)  child.gameObject.GetComponent<Collider>().enabled=false;
			DisableColliderRecursively(child);
		}
	}
	
	static public void SetMat2DiffuseRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.GetComponent<Renderer>()!=null){
				foreach(Material mat in child.GetComponent<Renderer>().materials)  
					mat.shader=Shader.Find( "Diffuse" );
			}
			//recurse.
			SetMat2DiffuseRecursively(child);
		}
	}
	
	static public void SetMat2AdditiveRecursively(Transform root){
		foreach(Transform child in root) {
			if(child.GetComponent<Renderer>()!=null){
				foreach(Material mat in child.GetComponent<Renderer>().materials)  
					mat.shader=Shader.Find("Particles/Additive");
			}
			//recurse.
			SetMat2AdditiveRecursively(child);
		}
	}
	
	static public void SetAdditiveMatColorRecursively(Transform root, Color color){
		foreach(Transform child in root) {
			if(child.GetComponent<Renderer>()!=null){
				foreach(Material mat in child.GetComponent<Renderer>().materials)  
					mat.SetColor("_TintColor", color);
			}
			//recurse.
			SetAdditiveMatColorRecursively(child, color);
		}
	}

	
}
