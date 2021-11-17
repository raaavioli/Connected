using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : GeneralComponent 
{
    private float requiredPower;

    public Light(float ohm, float watt) 
    {
        resistance = ohm;
        requiredPower = watt;
    }

    void Update() {
        //TODO: Animate if  watt is high enough
    }
}