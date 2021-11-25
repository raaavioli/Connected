using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : PowerSource {
    [SerializeField]
    float Voltage = 12f;

    [SerializeField]
    float Ampere = 1f;

    void Awake()
    {
        voltage = Voltage;
        current = Ampere;
    }
}
