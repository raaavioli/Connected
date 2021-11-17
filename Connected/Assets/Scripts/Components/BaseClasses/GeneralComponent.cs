using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GeneralComponent : MonoBehaviour {
    public Wire positive { get; set; }
    public Wire negative { get; set; }
    public float current { get; set; }
    public float resistance { get; protected set; }

    public static bool CheckConnection(Wire positive) {
        if (positive == null) return false;
        else return positive.positive != null;
    }
}
