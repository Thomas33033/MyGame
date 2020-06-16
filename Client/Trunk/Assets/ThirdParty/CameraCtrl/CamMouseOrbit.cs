using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/CamMouse")]
public class CamMouseOrbit : MonoBehaviour
{
    static float distance = 10.0f;

    public float ySpeed = 2.5f;
    public float distSpeed = 10.0f;

    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;
    public float distMinLimit = 5.0f;
    public float distMaxLimit = 50.0f;

    public float orbitDamping = 4.0f;
    public float distDamping = 4.0f;

    public float x = 0.0f;
    public float y = 0.0f;
    public float dist = distance;
    public float v = 0.0f;


    public Transform target ;


    public float xSpeed = 5;


    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation

        //if (rigidbody)
        //    rigidbody.freezeRotation = true;
    }
    
    private void LateUpdate()
    {
        if (!target || Input.touchCount > 0) return;


        if (Input.GetMouseButton(0))
        {
            //Screen.lockCursor = true;
            x += Input.GetAxis("Mouse X") * xSpeed;
            y -= Input.GetAxis("Mouse Y") * ySpeed;
            v += Input.GetAxis("Mouse X") * xSpeed - Mathf.Sign(v) * Time.deltaTime;
        }
        else if (Mathf.Abs(v) > 0.1f)
        {
            //Screen.lockCursor = false;
           // v = (Mathf.Abs(v) > 1.0f) ? (v * 0.1f) : v;
           // x += Mathf.Sign(v) * Time.deltaTime * xSpeed;
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * distSpeed;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        distance = Mathf.Clamp(distance, distMinLimit, distMaxLimit);

        dist = Mathf.Lerp(dist, distance, distDamping * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(y, x, 0), Time.deltaTime * orbitDamping);
        transform.position = transform.rotation * new Vector3(0.0f, 0.0f, -dist) + target.position;
    }

    public float ClampAngle(float a, float min, float max)
    {
        while (max < min) max += 360.0f;
        while (a > max) a -= 360.0f;
        while (a < min) a += 360.0f;

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










