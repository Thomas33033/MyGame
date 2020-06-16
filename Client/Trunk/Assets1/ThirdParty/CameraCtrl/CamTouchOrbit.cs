using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/CamTouchOrbit")]
public class CamTouchOrbit : MonoBehaviour
{
    public static float distance = 10.0f;

    public Transform target;
    public float height = 1.0f;

    public float xSpeed = 5.0f;
    public float ySpeed = 2.5f;
    public float distSpeed = 10.0f;
    public float panSpeed = 10.0f;

    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;
    public float distMinLimit = 5.0f;
    public float distMaxLimit = 50.0f;

    public float orbitDamping = 4.0f;
    public float distDamping = 4.0f;
    public float panDamping = 4.0f;

    private float x = 0.0f;
    private float y = 0.0f;
    private float dist = distance;
    private float v = 0.0f;
    private float lastTouchDist = 0.0f;
    private float panX = 0.0f;
    private float panZ = 0.0f;


    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation

        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.freezeRotation = true;
    }

    void LateUpdate()
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


        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                x += touch.deltaPosition.x * xSpeed;
                y -= touch.deltaPosition.y * ySpeed;
                v += touch.deltaPosition.x * xSpeed - Mathf.Sign(v) * Time.deltaTime;
            }
        }
        else if (Mathf.Abs(v) > 0.1f)
        {
            v = (Mathf.Abs(v) > 1.0f) ? (v * 0.1f) : v;
            x += Mathf.Sign(v) * Time.deltaTime * xSpeed;
        }

        if (Input.touchCount == 2)
        {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);

            var touchMoveDist = (touch1.deltaPosition - touch2.deltaPosition).sqrMagnitude;
            var touchTempDist = (touch1.position - touch2.position).magnitude;
            distance -= Mathf.Sign(touchTempDist - lastTouchDist) * distSpeed * touchMoveDist;
            lastTouchDist = touchTempDist;
        }

        if (Input.touchCount > 2)
        {
            var touchPan = Input.GetTouch(0);
            panX -= touchPan.deltaPosition.x * panSpeed;
            panZ -= touchPan.deltaPosition.y * panSpeed;
        }
        else
        {
            panX = Mathf.Lerp(panX, 0.0f, panDamping * Time.deltaTime);
            panZ = Mathf.Lerp(panZ, 0.0f, panDamping * Time.deltaTime);
        }

        Vector3 panVector = panX * transform.right + panZ * Vector3.Cross(transform.right, target.up);

        //distance -= Input.GetAxis("Mouse ScrollWheel") * distSpeed;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        distance = Mathf.Clamp(distance, distMinLimit, distMaxLimit);

        dist = Mathf.Lerp(dist, distance, distDamping * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
        transform.position = transform.rotation * new Vector3(0.0f, 0.0f, -dist) + target.position + panVector + target.up * height;
    }


    float ClampAngle(float a, float min, float max)
	{
	    while (max<min) max += 360.0f;
	    while (a > max) a -= 360.0f;
	    while (a<min) a += 360.0f;
	
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

}




