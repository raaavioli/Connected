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
    private bool _positive = true;
    public bool positive { get; private set; }
    [SerializeField]
    private GeneralComponent associatedComponent;

    private Wire connectedWire;
    private MeshRenderer meshRenderer;
    private Transform connectionPoint;
    private Connector connectedConnector;

	private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        connectionPoint = transform.Find("ConnectionPoint");

        positive = _positive;

        if (positive) {
            meshRenderer.material = positiveMaterial;
		} else {
            meshRenderer.material = negativeMaterial;
		}
	}

	public bool InitiateConnection(Connector newConnector) {
        if (IsEmpty()) {
            return Connect(newConnector);
        } else {
            return false;
		}
    }

	private bool Connect(Connector connector) {
        if (associatedComponent == null) {
            return false;
		}

        connectedConnector = connector;
        connectedWire = connectedConnector.associatedWire;

        connectedConnector.transform.position = connectionPoint.position;
        connectedConnector.transform.rotation = connectionPoint.rotation;
        
        if (positive) {
            associatedComponent.positive = connectedWire;
            connectedWire.SetEnd(associatedComponent, false);
		} else {
            associatedComponent.negative = connectedWire;
            connectedWire.SetEnd(associatedComponent, true);
		}

        return true;
	}

    public void Disconnect() {
        if (associatedComponent == null) {
            return;
        }

        if (positive) {
            associatedComponent.positive = null;
		} else {
            associatedComponent.negative = null;
		}
        connectedWire.ClearEnd(associatedComponent);
        connectedConnector = null;
        connectedWire = null;
	}

    public bool IsEmpty() {
        return connectedConnector == null;
	}
}
