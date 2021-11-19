using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WireRenderer))]
public class Wire : MonoBehaviour {
	[SerializeField]
	private Material positiveMaterial;
	[SerializeField]
	private Material negativeMaterial;
	[SerializeField]
	private Material neutralMaterial;
	[SerializeField]
	private Color positiveColor;
	[SerializeField]
	private Color negativeColor;
	[SerializeField]
	private Color neutralColor;

    public GeneralComponent positive { get; set; }
    public GeneralComponent negative { get; set; }

	private WireRenderer wireRenderer;
	private GameObject startPoint;
	private GameObject endPoint;

	private void Awake() {
		wireRenderer = GetComponent<WireRenderer>();
		startPoint = transform.Find("StartPoint").gameObject;
		endPoint = transform.Find("EndPoint").gameObject;
	}

	private void Update() {
		if (positive != null && negative != null) {
			wireRenderer.connected = true;
		} else {
			wireRenderer.connected = false;
		}
	}

	public void ConnectPositive(GameObject connector) {
		connector.GetComponent<MeshRenderer>().material = positiveMaterial;
		RecolorWire(connector, positiveColor);
	}

	public void ConnectNegative(GameObject connector) {
		connector.GetComponent<MeshRenderer>().material = negativeMaterial;
		RecolorWire(connector, negativeColor);
	}

	public void Disconnect(GameObject connector) {
		connector.GetComponent<MeshRenderer>().material = neutralMaterial;
		RecolorWire(connector, neutralColor);
	}

	private void RecolorWire(GameObject connector, Color color) {
		if (connector == startPoint) {
			wireRenderer.startColor = color;
		} else if (connector == endPoint) {
			wireRenderer.endColor = color;
		}
	}
}