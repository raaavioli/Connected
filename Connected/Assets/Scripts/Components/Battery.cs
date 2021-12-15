using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : GeneralComponent {
    [SerializeField]
    private float voltage;
    private int errorCount = 0, circuitCap = 500;

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
        WireEnd end;

        float resistanceSum = 0.0f;
        float voltageSum = 0.0f;

        if (CheckConnection(this, positive)) {
            end = positive.GetOtherEnd(this);
            if (end.polarity) {
                nextComponent = end.component;
            } else {
                nextComponent = null;
			}
        }
        errorCount = 0;
        while (nextComponent != this && nextComponent != null)
        {
            errorCount++;
            if (errorCount > circuitCap) {
                Debug.Log("cap reached in check");
                break;
            }
            resistanceSum += nextComponent.resistance;

            if (!CheckConnection(nextComponent, nextComponent.positive)) {
                break;
            }

            if (nextComponent.GetType() == typeof(Battery)) {
                Battery foundPowerSource = (Battery)nextComponent;
                foundPowerSources.Add(foundPowerSource);
                voltageSum += foundPowerSource.voltage;
            }
            end = nextComponent.positive.GetOtherEnd(nextComponent);
            if (end.polarity) {
                nextComponent = end.component;
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
        GeneralComponent nextComponent = this.positive.GetOtherComponent(this);

        errorCount = 0;
        while (nextComponent != this) {
            errorCount++;
            if (errorCount > circuitCap) {
                Debug.Log("cap reached in resolve");
                break;
            }
            nextComponent.current = current;
            nextComponent = nextComponent.positive.GetOtherComponent(nextComponent);
            nextComponent.positive.ShowCurrent();
        }
    }

    private void ResetCircuit() 
    { // Assumes broken circuit, trace both ways to resest currents to 0 and hide current shader in wires.
        GeneralComponent nextComponent = this;
        errorCount = 0;
        while (nextComponent != null)
        { // positive direction
            errorCount++;
            if (errorCount > circuitCap) {
                Debug.Log("cap reached in positive reset");
                break;
            }
            nextComponent.current = 0.0f;

            if (nextComponent.positive != null) {
                nextComponent.positive.HideCurrent();
            }

            if (CheckConnection(nextComponent, nextComponent.positive)) {
                nextComponent = nextComponent.positive.GetOtherComponent(nextComponent);
            } else { // No component existed on the other end of the wire.
                break;
            }
            if (nextComponent == this) break;
        }

        nextComponent = this;
        errorCount = 0;
        while (nextComponent != null)
        { // negative direction
            errorCount++;
            if (errorCount > circuitCap) {
                Debug.Log("Cap reached in negative´reset");
                break;
            }
            nextComponent.current = 0.0f;

            if (nextComponent.negative != null) {
                nextComponent.negative.HideCurrent();
            }

            if (CheckConnection(nextComponent, nextComponent.negative)) {
                nextComponent = nextComponent.negative.GetOtherComponent(nextComponent);
            } else { // No component existed on the other end of the wire.
                break;
            }
            if (nextComponent == this) break;
        }
    }
}