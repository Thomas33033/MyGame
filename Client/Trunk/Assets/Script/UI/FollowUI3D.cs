using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FollowUI3D : MonoBehaviour
{

    public Transform target;
    public Vector3 offsetPos = Vector3.zero;
    private float frontSize;

    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 offset;
    bool isLoad = false;

    private Vector3 mousePos;
    private Vector2 screenPos;


    private Camera uiCamera;
    void Start () {
        this.rectTransform = this.GetComponent<RectTransform>();
        canvas = GameObject.Find("UIRootHp").GetComponent<Canvas>();
        //canvas = this.GetComponent<Canvas>();
        //canvas.worldCamera = Camera.main;
        this.transform.localScale = Vector3.one * 0.015f;
    }
    
    void Update()
    {
        //screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rectTransform, screenPos, canvas.worldCamera, out mousePos))
        //{
        //    rectTransform.position = mousePos;
        //}
        //rectTransform.LookAt(Camera.main.transform, new Vector3(1,0,0));

        //3D血条
        rectTransform.position = target.transform.position;
        rectTransform.rotation = Camera.main.transform.rotation;
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
