using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : GeneralComponent 
{
    [SerializeField]
    private float requiredPower = 1.0f;
    [SerializeField]
    private float maximumPower = 10f;
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
        if (power < requiredPower)
            power = 0;
        // TODO: Break lamp if power > maximumPower
        dimming.SetIntensity(power / maximumPower);
    }
}