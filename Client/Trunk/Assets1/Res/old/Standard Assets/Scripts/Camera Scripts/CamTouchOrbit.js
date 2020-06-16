var target : Transform;
var distance = 10.0;
var height = 1.0;

var xSpeed = 5.0;
var ySpeed = 2.5;
var distSpeed = 10.0;
var panSpeed = 10.0;

var yMinLimit = -20.0;
var yMaxLimit = 80.0;
var distMinLimit = 5.0;
var distMaxLimit = 50.0;

var orbitDamping = 4.0;
var distDamping = 4.0;
var panDamping = 4.0;

private var x = 0.0;
private var y = 0.0;
private var dist = distance;
private var v = 0.0;
private var lastTouchDist = 0.0;
private var panX = 0.0;
private var panZ = 0.0;

function Start () 
	{
    var angles = transform.eulerAngles;
    x = angles.y;
    y = angles.x;
	
	// Make the rigid body not change rotation
	
   	if (GetComponent.<Rigidbody>())
		GetComponent.<Rigidbody>().freezeRotation = true;
	}
	
function LateUpdate () 
	{
     	//if running on Android, check for Menu/Home and exit
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            {
                Application.Quit();
                return;
            }
        }
        
    if (!target) return;
	
	
	if(Input.touchCount == 1){
		var touch = Input.GetTouch(0);
		if(touch.phase == TouchPhase.Moved){
			x += touch.deltaPosition.x * xSpeed;
			y -= touch.deltaPosition.y * ySpeed;
			v += touch.deltaPosition.x * xSpeed - Mathf.Sign(v) * Time.deltaTime;
		} 
	}else if(Mathf.Abs(v) > 0.1f){
		v = (Mathf.Abs(v) > 1.0f) ? (v*0.1) : v;
		x += Mathf.Sign(v) * Time.deltaTime * xSpeed;
	}
	
	if(Input.touchCount == 2){
		var touch1 = Input.GetTouch(0);
		var touch2 = Input.GetTouch(1);
		
		var touchMoveDist = (touch1.deltaPosition - touch2.deltaPosition).sqrMagnitude;
		var touchTempDist = (touch1.position - touch2.position).magnitude;
		distance -= Mathf.Sign(touchTempDist - lastTouchDist) * distSpeed * touchMoveDist;
		lastTouchDist = touchTempDist;
	}
	
	if(Input.touchCount > 2){
		var touchPan = Input.GetTouch(0);
		panX -= touchPan.deltaPosition.x * panSpeed;
		panZ -= touchPan.deltaPosition.y * panSpeed;
	}else{
		panX = Mathf.Lerp(panX,0.0,panDamping * Time.deltaTime);
		panZ = Mathf.Lerp(panZ,0.0,panDamping * Time.deltaTime);
	}
	
	var panVector : Vector3 = panX * transform.right + panZ * Vector3.Cross(transform.right, target.up);
	
	//distance -= Input.GetAxis("Mouse ScrollWheel") * distSpeed;
	
	y = ClampAngle(y, yMinLimit, yMaxLimit);		   
	distance = Mathf.Clamp(distance, distMinLimit, distMaxLimit);
	
	dist = Mathf.Lerp (dist, distance, distDamping * Time.deltaTime);		
	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
	transform.position = transform.rotation * Vector3(0.0, 0.0, -dist) + target.position + panVector + target.up * height;
	}	

	
function ClampAngle (a : float, min : float, max : float) : float 
	{
	while (max < min) max += 360.0;
	while (a > max) a -= 360.0;
	while (a < min) a += 360.0;
	
	if (a > max)
		{
		if (a - (max + min) * 0.5 < 180.0)
			return max;
		else
			return min;
		}
	else
		return a;
	}
