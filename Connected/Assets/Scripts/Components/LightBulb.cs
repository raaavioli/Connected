using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : GeneralComponent 
{
    [SerializeField]
    private float requiredPower;
    private Dimming dimming;

    public LightBulb(float ohm, float watt)
    {
        resistance = ohm;
        requiredPower = watt;
    }

	private void Awake() {
        dimming = GetComponent<Dimming>();
	}

	void Update() {
        float power = CalculatePower();
        dimming.SetIntensity(power / requiredPower);
    }
}