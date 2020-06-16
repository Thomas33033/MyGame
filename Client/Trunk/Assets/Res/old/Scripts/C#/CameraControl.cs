using UnityEngine;
using System.Collections;
//������갴��ö��  
enum MouseButton
{
    //������  
    MouseButton_Left = 0,
    //����Ҽ�  
    MouseButton_Right = 1,
    //����м�  
    MouseButton_Midle = 2
}

public class CameraControl : MonoBehaviour {

    public Transform targetT;
    public Transform thisT;
    public Camera cam;
    public float moveSpeed = 1;

    public bool isLock = false;
    public float curAngleH = 0;      //ˮƽ�Ƕ�
    public float curAngleV = 0;      //��ֱ�Ƕ�
    public float curDistance = 10;    //����Ŀ���ľ���

    private Vector3 _vec3TargetScreenSpace;// Ŀ���������Ļ�ռ�����  
    private Vector3 _vec3TargetWorldSpace;// Ŀ�����������ռ�����  
    private Vector3 _vec3MouseScreenSpace;// ������Ļ�ռ�����  
    private Vector3 _vec3Offset;// ƫ�� 

    //������ž�����ֵ  
    public float MaxDistance = 90f;
    public float MinDistance = 50f;
    //�����������  
    public float ZoomSpeed = 2F;
    public float Distance;
    public float y;

    private Vector3 offset;
    private Vector3 targetPosition;
    void Awake()
    {
		thisT=transform;
        CameraController.Instance.mainCamera = this.transform.Find("Main Camera").GetComponent<Camera>();
        cam = Camera.main;
        y = this.transform.position.y;

        targetPosition = targetT.position;
    }
	

	void Start ()
    {
        //�����㴥��  
        Input.multiTouchEnabled = true;
    }

    private Vector3 lastMousePos;
    private Vector3 lastWordPos;

    private bool resetPosition = false;
    private Vector3 curPosition;
    private Vector3 curWordSpace;
    private Vector3 wordSpaceDiff;
    void LateUpdate()
    {
        if (resetPosition)
        {
            transform.position += wordSpaceDiff;
            resetPosition = false;
        }
    }
    private Vector3 oldPosition;
    void Update ()
    {
        Debug.DrawLine(this.cam.transform.position,this.cam.transform.forward*3,Color.red);

        if (isLock) return;

        if (Const1.IsPointOverUIObject())
        {
            
            return;
        }

        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            
        }

        if (Input.GetMouseButtonDown((int)MouseButton.MouseButton_Left))
        {
            _vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(transform.position);
            lastMousePos = Input.mousePosition;
            Vector3 p1 = new Vector3(lastMousePos.x, lastMousePos.y, _vec3TargetScreenSpace.z);
            lastWordPos = Camera.main.ScreenToWorldPoint(p1);

            //lastMousePos = Input.mousePosition;
            //_vec3TargetScreenSpace = Camera.main.WorldToScreenPoint(targetT.position);
            //_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);
            //_vec3Offset = targetT.position - Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace);
            //y = targetT.position.y;

        }
        else if (Input.GetMouseButtonUp((int)MouseButton.MouseButton_Left))
        {
            lastMousePos = Vector3.zero;
        }
        else if (Input.GetMouseButton((int)MouseButton.MouseButton_Left))
        {
            if (lastMousePos != Input.mousePosition)
            {
                Vector3 p2 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);
                curWordSpace = Camera.main.ScreenToWorldPoint(p2);
                wordSpaceDiff = lastWordPos - curWordSpace;
                wordSpaceDiff.y = 0;
                lastMousePos = Input.mousePosition;
                transform.position += wordSpaceDiff;

                //_vec3MouseScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _vec3TargetScreenSpace.z);
                //_vec3TargetWorldSpace = Camera.main.ScreenToWorldPoint(_vec3MouseScreenSpace) + _vec3Offset;
                //_vec3TargetWorldSpace.y = y;
                //targetT.position = _vec3TargetWorldSpace;
                
                //Vector3 temp = lastMousePos - Input.mousePosition;
                //Vector3 rotationVector = Quaternion.Euler(this.transform.eulerAngles) * new Vector3(temp.x, 0, temp.y);
                //rotationVector.y = 0;
                //this.transform.localPosition += rotationVector * Time.deltaTime * moveSpeed;
                //lastMousePos = Input.mousePosition;
            }
        }

        //����м�ƽ��  
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            //����������  
            Distance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            //��������ӿ�
            cam.fieldOfView = Distance;
        }

       // this.transform.position = targetT.position - offset;

    }


   

}
