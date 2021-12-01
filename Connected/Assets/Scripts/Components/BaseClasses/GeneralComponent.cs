using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralComponent : MonoBehaviour {
    public Wire positive { get; set; }
    public Wire negative { get; set; }
    public float current { get; set; }
    public float resistance { get; protected set; }

    public static bool CheckConnection(Wire wire, bool positiveDirection = true) {
        if (wire == null) {
            return false;
        } else if (positiveDirection) {
            return wire.positive != null;
        } else {
            return wire.negative != null;
        }
    }

    protected float CalculateVoltage() {
        return resistance * current;
	}

    protected float CalculatePower() {
        return CalculateVoltage() * current;
	}
}
