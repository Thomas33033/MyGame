using UnityEngine;
using System.Collections;

public class CustomCar : MonoBehaviour {
	public Transform[] circles;
	
	bool selectCol=false;
	float radTime=20;
	public Material carPaint;
	Color lastA,lastB;
	public Camera uic;
	
	Vector3 pressPos;
	
	// Use this for initialization
	void Start () {
		lastA=Color.white;
		lastB=Color.white;
		carPaint.SetColor("_RimCol1a",lastA);
		carPaint.SetColor("_RimCol1b",lastB);
		carPaint.SetColor("_RimCol2a",lastA);
		carPaint.SetColor("_RimCol2b",lastB);
	}
	
	// Update is called once per frame
	void Update () {
	
		
		if(radTime<6){
			carPaint.SetFloat("_Radius",radTime);
			radTime+=7*Time.deltaTime;
		}
		if(radTime>5){
			if(Input.GetMouseButtonDown(0)){
				pressPos=Input.mousePosition;
			}
			
			if(Input.GetMouseButtonUp(0)){
				if((Input.mousePosition-pressPos).magnitude>20)
					return;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit,100)){
					if(hit.transform.gameObject.name=="car"){
						if(!selectCol){
							circles[0].parent.localPosition = Camera.main.WorldToScreenPoint(hit.point)-new Vector3(Screen.width/2,Screen.height/2,0);
							carPaint.SetFloat("_PosX",hit.point.x);
							carPaint.SetFloat("_PosY",hit.point.y);
							carPaint.SetFloat("_PosZ",hit.point.z);
							carPaint.SetColor("_RimCol1a",lastA);
							carPaint.SetColor("_RimCol1b",lastB);
							selectCol=true;	
							StartCoroutine(ShowCircle());
							uic.enabled=true;
						}
						else{
							selectCol=false;
							StartCoroutine(HideCircle());

						}
					}
				}
			}
		}
		
	}
	
	IEnumerator ShowCircle(){
		for(int i=0;i<8;i++){
			yield return new WaitForSeconds(0.07f);
			circles[i].GetComponent<Animation>().Play("Show");
		}
		uic.enabled=true;
	}
	IEnumerator HideCircle(){
		for(int i=0;i<8;i++){
			yield return new WaitForSeconds(0.07f);
			circles[i].GetComponent<Animation>().Play("Hide");
		}
	}
	
	public void ChangeColor(int id){
	//	print ("ChangeColor");
		if(radTime<4)
			return;
		radTime=0;
		switch(id){
		case 1:	
			lastA=new Color(1,0,0,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(1,0.5f,0,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 2:
			lastA=new Color(1,0,1,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0.1f,0.5f,1f,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 3:
			lastA=new Color(0,0,0f,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0,0f,0,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 4:
			lastA=new Color(0,0,0.8f,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0,0.5f,0,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 5:
			lastA=new Color(0,0.6f,0.4f,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0.3f,0.3f,0.3f,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 6:
			lastA=new Color(1,1,1,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0.85f,0.85f,0.85f,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 7:
			lastA=new Color(0.7f,0.7f,0.7f,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0.2f,0.2f,0.2f,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		case 8:
			lastA=new Color(0.2f,0.1f,0f,1);
			carPaint.SetColor("_RimCol2a",lastA);
			lastB=new Color(0.5f,0.5f,0.3f,1);
			carPaint.SetColor("_RimCol2b",lastB);
			break;
		}
		carPaint.SetFloat("_Radius",radTime);
		selectCol=false;
		StartCoroutine(HideCircle());
		uic.enabled=false;
	}
}
