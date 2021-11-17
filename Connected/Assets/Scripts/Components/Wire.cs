using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WireRenderer))]
public class Wire : MonoBehaviour {
    public GeneralComponent positive { get; set; }
    public GeneralComponent negative { get; set; }

	private WireRenderer wireRenderer;

	private void Awake() {
		wireRenderer = GetComponent<WireRenderer>();
	}

	private void Update() {
		if (positive != null && negative != null) {
			wireRenderer.connected = true;
		} else {
			wireRenderer.connected = false;
		}
	}
}