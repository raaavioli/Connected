using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerSource : GeneralComponent {
    public float voltage { get; protected set; }

    private void Start()
    {
        CircuitManager.AddPowerSource(this);
    }

    private void OnDestroy()
    {
        CircuitManager.RemovePowerSource(this);
    }

    public List<PowerSource> CheckCircuit() // Returns other power sources that is in the circuit for the circuit manager.
    {
        GeneralComponent nextComponent = null;
        List<PowerSource> foundPowerSources = new List<PowerSource>();
        foundPowerSources.Add(this);

        float resistanceSum = 0.0f;
        float voltageSum = 0.0f;

        if (CheckConnection(positive)) {
            nextComponent = positive.positive;
        }

        while (nextComponent != this)
        {
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
        voltageSum += this.voltage; resistanceSum += this.resistance;
        if (nextComponent == this) {
            ResolveCircuit(resistanceSum, voltageSum);
        }

        return foundPowerSources;
    }

    private void ResolveCircuit(float resistance, float voltage) // Can safely assume closed circuit
    {
        float current = voltage / (resistance + Mathf.Epsilon); //TODO: Explode powersource or whatever, when current is near-infinite.
        this.current = current;
        this.positive.ShowCurrent();
        GeneralComponent nextComponent = this.positive.positive;

        while (nextComponent != this) {
            nextComponent.current = current;
            nextComponent.positive.ShowCurrent();
            nextComponent = nextComponent.positive.positive;
        }
    }
}