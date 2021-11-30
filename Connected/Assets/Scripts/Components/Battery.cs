using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : GeneralComponent {
    [SerializeField]
    private float voltage;

    private int MAX_CIRCUIT_SIZE = 1000;

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

        while (nextComponent != this && nextComponent != null)
        {
            resistanceSum += nextComponent.resistance;

            if (nextComponent.GetType() == typeof(Battery)) {
                Battery foundPowerSource = (Battery)nextComponent;
                foundPowerSources.Add(foundPowerSource);
                voltageSum += foundPowerSource.voltage;
            }

            if (CheckConnection(nextComponent.positive)) {
                nextComponent = nextComponent.positive.positive;
            } else {
                break;
            }
        }
        voltageSum += this.voltage; resistanceSum += this.resistance;
        if (nextComponent == this) {
            ResolveCircuit(resistanceSum, voltageSum);
        } else {
            ResetCircuit();
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

    private void ResetCircuit() { // Assumes broken circuit, trace both ways to resest currents to 0 and hide current shader in wires.
        // Trace in positive direction.
        GeneralComponent nextComponent = this;
        for (int i = 0; i < MAX_CIRCUIT_SIZE; ++i) {
            // Reset current.
            nextComponent.current = 0.0f;
            
            if (nextComponent.positive != null) { // If there is a wire connected.
                nextComponent.positive.HideCurrent();
                if (nextComponent.positive.positive != null) { // If the wire is connected to an additional component.
                    nextComponent = nextComponent.positive.positive;
				} else { // There was no next component, circuit was broken here.
                    break;
				}
            } else { // There was no next wire, circuit was broken here.
                break;
			}
        }

        // Trace in negative direction.
        nextComponent = this;
        for (int i = 0; i < MAX_CIRCUIT_SIZE; ++i) {
            // Reset current.
            nextComponent.current = 0.0f;

            if (nextComponent.negative != null) { // If there is a wire connected.
                nextComponent.negative.HideCurrent();
                if (nextComponent.negative.negative != null) { // If the wire is connected to an additional component.
                    nextComponent = nextComponent.negative.negative;
                } else { // There was no next component, circuit was broken here.
                    break;
                }
            } else { // There was no next wire, circuit was broken here.
                break;
            }
        }
    }
}