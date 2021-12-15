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
    private float ohm = 5f;

    private Dimming dimming;
    private bool active;
    private AudioSource audioSource;

	private void Awake() {
        dimming = GetComponent<Dimming>();
        audioSource = GetComponent<AudioSource>();
        resistance = ohm;
        active = false;
    }

	void Update() {
        float power = CalculatePower();
        if (power < requiredPower) {
            power = 0;
            ModifySound(false);
        }
        else {
            ModifySound(true);
        }
        // TODO: Break lamp if power > maximumPower
        dimming.SetIntensity(power / maximumPower);
    }

    private void ModifySound(bool turnOn) {
        if(!active && turnOn) {
            active = true;
            audioSource.Play();
        }
        else if(active && !turnOn) {
            active = false;
            audioSource.Stop();
        }
    }
}