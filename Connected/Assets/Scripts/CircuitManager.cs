using System.Collections.Generic;
using UnityEngine;

public class CircuitManager : MonoBehaviour 
{
    private List<Battery> batteries = new List<Battery>();
    private static CircuitManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public static void AddPowerSource(Battery battery)
    {
        if (instance != null) {
            instance.batteries.Add(battery);
        }
    }

    public static void RemovePowerSource(Battery battery)
    {
        if (instance != null) {
            instance.batteries.Remove(battery);
        }
    }

    public static void TraceCircuits() {
        List<Battery> foundPowerSources = new List<Battery>();
        
        foreach (Battery battery in instance.batteries) {
            if (foundPowerSources.Contains(battery)) {
                continue;
            } else {
                foundPowerSources.AddRange(battery.CheckCircuit());
            }
        }
    }
}