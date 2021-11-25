using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WireRenderer))]
public class Wire : MonoBehaviour {

    public GeneralComponent positive { get; set; }
    public GeneralComponent negative { get; set; }

	private WireRenderer wireRenderer;
	private Connector startConnector;
	private Connector endConnector;

	private void Awake() {
		wireRenderer = GetComponent<WireRenderer>();
		startConnector = transform.Find("StartPoint").GetComponent<Connector>();
		endConnector = transform.Find("EndPoint").GetComponent<Connector>();
	}

	private void Update() {
		if (positive == null || negative == null) {
			wireRenderer.connected = false;
		}
	}

	public void ShowCurrent() {
		wireRenderer.connected = true;
	}

	public void RecolorWire(Connector connector, Color color) {
		if (connector == startConnector) {
			wireRenderer.startColor = color;
		} else if (connector == endConnector) {
			wireRenderer.endColor = color;
		}
	}
}