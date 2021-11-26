using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : PowerSource {
    [SerializeField]
    float ampere = 1f;

    void Awake()
    {
        current = ampere;
    }
}
