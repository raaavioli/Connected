using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : GeneralComponent 
{
    private float requiredPower;

    public LightBulb(float ohm, float watt)
    {
        resistance = ohm;
        requiredPower = watt;
    }

    void Update() {
        //TODO: Animate if  watt is high enough
    }
}