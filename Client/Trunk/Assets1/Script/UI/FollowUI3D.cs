using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FollowUI3D : MonoBehaviour
{
    private Transform target;
    public Vector3 offsetPos = Vector3.zero;
    private float frontSize;

    private GameObject canvas;
    // Use this for initialization
    bool isLoad = false;
	void Start () {
        if (isLoad)
        {
            return;
        }
        //isLoad = true;
        //this.transform.localScale = Vector3.one;
        //canvas = GameObject.Instantiate(prefab).gameObject;
        //canvas.gameObject.SetActive(true);
        //target = canvas.transform;
        //canvas.transform.parent = this.transform;
        //canvas.transform.localPosition = Vector3.zero;
        //canvas.transform.localEulerAngles = Vector3.zero;
        //canvas.transform.localScale = Vector3.one;
	}

    void Update()
    {
        //text.gameObject.layer = DataManager.Instance.UI3D_S;
        //text.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        //canvas.transform.parent = DataManager.Instance.canvas_S.transform;
        this.gameObject.transform.LookAt(Camera.main.transform);
    }


	// Update is called once per frame
    //void FixedUpdate () 
    //{
    //    return;
    //    if (target != null)
    //    {
    //        //Vector3 player2DPosition = Camera.main.WorldToScreenPoint(transform.position);
    //        ////血条超出屏幕就不显示  
    //        //if (player2DPosition.x > Screen.width || player2DPosition.x < 0 || player2DPosition.y > Screen.height || player2DPosition.y < 0)
    //        //{
    //        //    canvas.gameObject.SetActive(false);
    //        //}
    //        //else
    //        //{
    //            canvas.gameObject.SetActive(true);
    //            float distance = Camera.main.orthographicSize;
    //            float scale = distance / (3.7f-0.4f) * 0.5f;
    //            canvas.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    //       // } 
    //    }
    //}
}
