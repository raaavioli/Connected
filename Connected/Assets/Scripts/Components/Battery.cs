using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : GeneralComponent {
    [SerializeField]
    private float voltage;

    private void Start()
    {
        CircuitManager.AddPowerSource(this);
    }

    private void OnDestroy()
    {
        CircuitManager.RemovePowerSource(this);
    }

    public List<Battery> CheckCircuit() // Returns other power sources that is in the circuit for the circuit manager.
    {
        GeneralComponent nextComponent = null;
        List<Battery> foundPowerSources = new List<Battery>();
        foundPowerSources.Add(this);

        float resistanceSum = 0.0f;
        float voltageSum = 0.0f;

        if (CheckConnection(positive)) {
            nextComponent = positive.positive;
        }

        while (nextComponent != this)
        {
            resistanceSum += nextComponent.resistance;

            if (!CheckConnection(nextComponent.positive)) break;

            if (nextComponent.GetType() == typeof(Battery)) {
                Battery foundPowerSource = (Battery)nextComponent;
                foundPowerSources.Add(foundPowerSource);
                voltageSum += foundPowerSource.voltage;
            } else if (nextComponent.GetType() == typeof(Splitter)) {
                float splitterResistance;
                Splitter foundSplitter = (Splitter)nextComponent;

                (splitterResistance, nextComponent) = foundSplitter.CheckSplitter();
                if (splitterResistance == 0 && nextComponent == null) { // if splitter wasn't closed
                    break;
                } else  {
                    resistanceSum += splitterResistance;
                }
            }
            nextComponent = nextComponent.positive.positive;
        }
        voltageSum += this.voltage; resistanceSum += this.resistance;
        if (nextComponent == this) {
            ResolveCircuit(resistanceSum, voltageSum);
        } else Debug.Log("Broken");

        return foundPowerSources;
    }

    private void ResolveCircuit(float resistance, float voltage) // Can safely assume closed circuit
    {
        float current = voltage / (resistance + Mathf.Epsilon); //TODO: Explode powersource or whatever, when current is near-infinite.
        this.current = current;
        this.positive.ShowCurrent();
        GeneralComponent nextComponent = this.positive.positive;

        while (nextComponent != this) {
            if (nextComponent.GetType() == typeof(Splitter)) {
                Splitter foundSplitter = (Splitter)nextComponent;
                nextComponent = foundSplitter.ResolveSplitter(current);
            } else {
                nextComponent.current = current;
                nextComponent = nextComponent.positive.positive;
                nextComponent.positive.ShowCurrent();
            }
        }
    }
}