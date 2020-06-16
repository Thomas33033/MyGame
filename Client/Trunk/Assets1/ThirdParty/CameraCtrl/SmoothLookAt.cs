using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Look At")]

public class SmoothLookAt : MonoBehaviour
{
    Transform target;
    float damping = 6.0f;
    bool smooth = true;

    void Start()
    {
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody)
            rigidbody.freezeRotation = true;
    }

    void LateUpdate()
    {
        if (target)
        {
            if (smooth)
            {
                // Look at and dampen the rotation
                var rotation = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            }
            else
            {
                // Just lookat
                transform.LookAt(target);
            }
        }
    }
}








