using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : PowerSource {

    public Battery(float volt, float ampere) {
        voltage = volt;
        current = ampere;
    }
}
