using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : GeneralComponent 
{
    [SerializeField]
    private float requiredPower = 1.0f;
    [SerializeField]
    private float maximumPower = 10f;
    [SerializeField]
    private float Ohm = 5f;
    private Dimming dimming;

	private void Awake() {
        dimming = GetComponent<Dimming>();
        resistance = Ohm;
    }

	void Update() {
        float power = CalculatePower();
        if (power < requiredPower)
            power = 0;
        // TODO: Break lamp if power > maximumPower
        dimming.SetIntensity(power / maximumPower);
    }
}