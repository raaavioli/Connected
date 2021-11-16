using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralComponent : MonoBehaviour {
    public Wire positive { get; set; }
    public Wire negative { get; set; }
    public float current { get; set; }
    public float resistance { get; protected set; }

    public static bool CheckConnection(Wire positive) {
        if (positive == null) return false;
        else return positive.positive != null;
    }
}

public abstract class PowerSource : GeneralComponent {
    public float voltage { get; protected set; }

    public List<PowerSource> CheckCircuit() // Returns other power sources that is in the circuit for the circuit manager.
    {
        GeneralComponent nextComponent = null;
        List<PowerSource> foundPowerSources = new List<PowerSource>();
        float resistanceSum = 0.0f;
        float voltageSum = 0.0f;

        if (CheckConnection(positive)) {
            nextComponent = positive.positive;
        }

        while (nextComponent != this) {
            resistanceSum += nextComponent.resistance;

            if (nextComponent.GetType() == typeof(PowerSource)) {
                PowerSource foundPowerSource = (PowerSource)nextComponent;
                foundPowerSources.Add(foundPowerSource);
                voltageSum += foundPowerSource.voltage;
            }

            if (CheckConnection(nextComponent.positive)) {
                nextComponent = positive.positive;
            } else {
                break;
            }
        }
        voltageSum += this.voltage;
        if (nextComponent == this) ResolveCircuit(resistanceSum, voltageSum);

        return foundPowerSources;
    }

    private void ResolveCircuit(float resistance, float voltage) // Can safely assume closed circuit
    {
        float current = voltage / (resistance + Mathf.Epsilon); //TODO: Explode power source or whatever, when current is near-infinite.
        this.current = current;
        GeneralComponent currentComponent = this.positive.positive;

        while (currentComponent != this) {
            currentComponent.current = current;
            currentComponent = currentComponent.positive.positive;
        }
    }
}
