using System.Collections.Generic;
using UnityEngine;

public class CircuitManager : MonoBehaviour 
{
    private List<PowerSource> powerSources = new List<PowerSource>();
    private static CircuitManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Update()
    {
        // Should be called on change in circuit. 
        // IE new wire/component connected or lightswitch flicked or whatever

        TraceCircuits();
    }

    public static void AddPowerSource(PowerSource powerSource)
    {
        if (instance != null) {
            instance.powerSources.Add(powerSource);
        }
    }

    public static void RemovePowerSource(PowerSource powerSource)
    {
        if (instance != null) {
            instance.powerSources.Remove(powerSource);
        }
    }

    private void TraceCircuits() {
        List<PowerSource> foundPowerSources = new List<PowerSource>();
        
        foreach (PowerSource powerSource in powerSources) {
            if (foundPowerSources.Contains(powerSource)) {
                continue;
            } else {
                foundPowerSources.AddRange(powerSource.CheckCircuit());
            }
        }
    }
}