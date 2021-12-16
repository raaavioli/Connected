using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WireRenderer))]
public class Wire : MonoBehaviour {

	// positive = true, negative == false
	private WireEnd pointA = new WireEnd(), pointB = new WireEnd();

	private WireRenderer wireRenderer;
	private Connector startConnector;
	private Connector endConnector;

	[HideInInspector]
	public WireSpawner spawner;

	private void OnDestroy()
    {
		if (startConnector != null)
        {
			Destroy(startConnector.gameObject);
        }

		if (endConnector != null)
        {
			Destroy(endConnector.gameObject);
        }

		spawner.currentWires -= 1;
	}

	private void Awake() {
		wireRenderer = GetComponent<WireRenderer>();
		startConnector = transform.Find("StartPoint").GetComponent<Connector>();
		endConnector = transform.Find("EndPoint").GetComponent<Connector>();
	}

	public void ShowCurrent() {
		wireRenderer.connected = CalculateCurrentDirection();
	}

	public void HideCurrent() {
		wireRenderer.connected = 0;
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
	public WireEnd GetOtherEnd(GeneralComponent start) {
		if (start == null) {
			return new WireEnd();
		}

		if (pointA.component == start) {
			return pointB;
		} else if (pointB.component == start) {
			return pointA;
		} else {
			return new WireEnd();
		}
	}

	public GeneralComponent GetOtherComponent(GeneralComponent start) {
		return this.GetOtherEnd(start).component;
	}

	public void SetEnd(GeneralComponent component, bool polarity) {
		if (pointA.component == null) {
			pointA = new WireEnd(component, polarity);
		} else if (pointB.component == null) {
			pointB = new WireEnd(component, polarity);
		} else {
			Debug.LogError("SetEnd broke");
		}
	}

	public void ClearEnd(GeneralComponent component) {
		if (pointA.component == component) {
			pointA.component = null;
		} else if (pointB.component == component) {
			pointB.component = null;
		}
	}
}

public class WireEnd {
	public GeneralComponent component { get; set; }
	public bool polarity { get; set; }

	public WireEnd() { }
	public WireEnd(GeneralComponent component, bool isPositive) {
		polarity = isPositive;
		this.component = component;
	}
}