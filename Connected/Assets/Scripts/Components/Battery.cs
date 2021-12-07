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

        while (nextComponent != this && nextComponent != null)
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
                if (splitterResistance == 0 && nextComponent == null) { // if splitter wasn't closed correctly
                    break;
                } else  {
                    resistanceSum += splitterResistance;
                }
            } else if (nextComponent.GetType() == typeof(Combiner)) { // if battery is inside of a splitter-combiner.
                // Temporary error message
                Debug.Log("Battery not allowed to be coupled inside of a splitter-combiner.");
                break;
            }
            nextComponent = nextComponent.positive.positive;
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

    private void ResetCircuit() 
    { // Assumes broken circuit, trace both ways to resest currents to 0 and hide current shader in wires.
        GeneralComponent nextComponent = this;
        while (nextComponent != null)
        { // positive direction
            nextComponent.current = 0.0f;

            if (nextComponent.positive != null) {
                nextComponent.positive.HideCurrent();
            }

            if (nextComponent.GetType() == typeof(Splitter)) {  //  Go into splitter logic.
                Splitter foundSplitter = (Splitter)nextComponent;
                nextComponent = foundSplitter.ResetSplitter();
                if (nextComponent == null) { // If splitter found the break
                    break;
                }
            } 

            if (CheckConnection(nextComponent.positive)) {
                nextComponent = nextComponent.positive.positive;
            } else { // No component existed on the other end of the wire.
                break;
            }
        }

        nextComponent = this;
        while (nextComponent != null) 
        { // negative direction
            nextComponent.current = 0.0f;

            if (nextComponent.negative != null) {
                nextComponent.negative.HideCurrent();
            }

            if (nextComponent.GetType() == typeof(Combiner)) {  //  Go into splitter logic.
                Combiner foundCombiner = (Combiner)nextComponent;
                nextComponent = foundCombiner.ResetCombiner();
                if (nextComponent == null) { // If Combiner found the break
                    break;
                }
            }

            if (CheckConnection(nextComponent.negative, false)) {
                nextComponent = nextComponent.negative.negative;
            } else { // No component existed on the other end of the wire.
                break;
            }
        }
    }
}