var target : Transform;
var distance = 10.0;

var xSpeed = 5;
var ySpeed = 2.5;
var distSpeed = 10.0;

var yMinLimit = -20.0;
var yMaxLimit = 80.0;
var distMinLimit = 5.0;
var distMaxLimit = 50.0;

var orbitDamping = 4.0;
var distDamping = 4.0;

private var x = 0.0;
private var y = 0.0;
private var dist = distance;
public var v = 0.0;

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
    if (!target || Input.touchCount > 0) return;
	
	
	if(Input.GetMouseButton(0)){
		//Screen.lockCursor = true;
		x += Input.GetAxis("Mouse X") * xSpeed;
		y -= Input.GetAxis("Mouse Y") * ySpeed;
		v += Input.GetAxis("Mouse X") * xSpeed - Mathf.Sign(v) * Time.deltaTime; 
	}else if(Mathf.Abs(v) > 0.1f){
		//Screen.lockCursor = false;
		v = (Mathf.Abs(v) > 1.0f) ? (v*0.1) : v;
		x += Mathf.Sign(v) * Time.deltaTime * xSpeed;
	}
	
	distance -= Input.GetAxis("Mouse ScrollWheel") * distSpeed;
	
	y = ClampAngle(y, yMinLimit, yMaxLimit);		   
	distance = Mathf.Clamp(distance, distMinLimit, distMaxLimit);
	
	dist = Mathf.Lerp (dist, distance, distDamping * Time.deltaTime);		
	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
	transform.position = transform.rotation * Vector3(0.0, 0.0, -dist) + target.position;
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
