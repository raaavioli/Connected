using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralComponent : MonoBehaviour {
    public Wire positive { get; set; }
    public Wire negative { get; set; }
    public float current { get; set; }
    public float resistance { get; protected set; }

    public static bool CheckConnection(GeneralComponent thisComponent, Wire wire) {
        if (wire == null) {
            return false;
        } else {
            return wire.GetOtherComponent(thisComponent) != null;
		}
    }

    protected float CalculateVoltage() {
        return resistance * current;
	}

    protected float CalculatePower() {
        return CalculateVoltage() * current;
	}
}
