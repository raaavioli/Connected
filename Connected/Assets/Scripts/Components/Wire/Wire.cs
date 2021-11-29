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
			wireRenderer.connected = 0;
		}
	}

	public void ShowCurrent() {
		wireRenderer.connected = CalculateCurrentDirection();
	}

	public void RecolorWire(Connector connector, Color color) {
		if (connector == startConnector) {
			wireRenderer.startColor = color;
		} else if (connector == endConnector) {
			wireRenderer.endColor = color;
		}
	}

	private int CalculateCurrentDirection() {
		if (startConnector.GetPolarity() == 1 && endConnector.GetPolarity() == -1) {
			// If from start to end, return 1, signifying that the current flows "in the same direcion" as the wire.
			return 1;
		} else if (startConnector.GetPolarity() == -1 && endConnector.GetPolarity() == 1) {
			// Else if from end to start, return -1, signifying that the current flows "in the opposite direcion" as the wire.
			return -1;
		} else {
			// Otherwise, something was not as it should be and the connection is broken.
			return 0;
		}
	}
}