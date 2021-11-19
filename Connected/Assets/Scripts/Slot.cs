using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private Material positiveMaterial;
    [SerializeField]
    private Material negativeMaterial;
    [SerializeField]
    private bool positive = true;
    [SerializeField]
    private GeneralComponent associatedComponent;

    private Wire connectedWire;
    private MeshRenderer meshRenderer;

	private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();

		if (positive) {
            meshRenderer.material = positiveMaterial;
		} else {
            meshRenderer.material = negativeMaterial;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Connector")) {
            if (connectedWire != null) {
                Disconnect();
            }
            Connect(other.transform.parent.GetComponent<Wire>());
		}
	}

	private void Connect(Wire wire) {
        connectedWire = wire;
        
        if (positive) {
            associatedComponent.positive = wire;
            connectedWire.negative = associatedComponent;
		} else {
            associatedComponent.negative = wire;
            connectedWire.positive = associatedComponent;
		}
	}

    private void Disconnect() {
        if (positive) {
            associatedComponent.positive = null;
            connectedWire.negative = null;
		} else {
            associatedComponent.negative = null;
            connectedWire.positive = null;
		}

        connectedWire = null;
	}
}
