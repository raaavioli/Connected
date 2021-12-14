using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TunerWheel : MonoBehaviour
{
    public float GetValue01()
    {
        // Wheel rotation angle is -270 to 0, or simply 90 to 360.
        // Mapping from 90 -> 0 and 360 -> 1
        return Mathf.Clamp01(1 - (360f - transform.localEulerAngles.z) / 270f);
    }

    void Update()
    {
        float rot = transform.localEulerAngles.z;
        if (rot <= 90f && rot > 45f)
            rot = 91f;
        if (rot > 359f || rot <= 45f)
            rot = 359f;
        transform.localEulerAngles = new Vector3(0, 0, rot);
    }
}
