using UnityEngine;

public class CircuitManager : MonoBehaviour {
    private PowerSource[] powerSources;

    private void Update() {
        TraceCircuits();
    }

    private void TraceCircuits() {
        List<PowerSource> foundPowerSources = new List<PowerSource>();
        
        foreach (PowerSource powerSource in powerSources) {
            if (foundPowerSources.Contains(powerSource)) {
                continue;
            }

            foundPowerSources.AddRange(powerSource.CheckCircuit());
        }
    }
}