using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Connector : MonoBehaviour {

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

	public Wire associatedWire { get; private set; }

    private MeshRenderer meshRenderer;
	private Interactable interactable;
	private Slot connectedSlot = null;
	private Rigidbody rb;
	private SphereCollider trigger;
	private BoxCollider boxCollider;
	private void Awake() {
		meshRenderer = GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody>();
		trigger = GetComponent<SphereCollider>();
		interactable = GetComponent<Interactable>();
		boxCollider = GetComponent<BoxCollider>();
	}

	private void OnEnable() {
		interactable.onAttachedToHand += GrabDisconnect;
	}

	private void OnDisable() {
		interactable.onAttachedToHand -= GrabDisconnect;
	}

	private void GrabDisconnect(Hand hand) {
		if (connectedSlot != null) {
			DisconnectionActions();
		}
	}

	private void Start() {
        associatedWire = transform.parent.GetComponent<Wire>();
    }

	private void Update() {
		if (!IsHeld() && connectedSlot == null) {
			rb.isKinematic = false;
			transform.parent = associatedWire.transform;
			boxCollider.isTrigger = false;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Slot")) {
            connectedSlot = other.GetComponent<Slot>();
			if (connectedSlot.IsEmpty()) {
				ConnectionActions();
			}
		}
	}

	private void ConnectionActions() {
		if (connectedSlot.InitiateConnection(this)) {
			if (IsHeld()) {
				interactable.attachedToHand.DetachObject(gameObject);
			}

			bool positive = connectedSlot.positive;
			rb.isKinematic = true;
			transform.parent = connectedSlot.transform;
			trigger.enabled = false;
			boxCollider.isTrigger = true;

			meshRenderer.material = positive ? positiveMaterial : negativeMaterial;
			associatedWire.RecolorWire(this, positive ? positiveColor : negativeColor);

			CircuitManager.TraceCircuits();
		}
	}

	private void DisconnectionActions() {
		connectedSlot.Disconnect();
		connectedSlot = null;
		StartCoroutine(DelayEnableTrigger());

		meshRenderer.material = neutralMaterial;
		associatedWire.RecolorWire(this, neutralColor);

		CircuitManager.TraceCircuits();
	}

	private bool IsHeld() {
		return interactable != null && interactable.attachedToHand != null;
	}

	private IEnumerator DelayEnableTrigger() {
		yield return new WaitForSeconds(0.5f);
		trigger.enabled = true;
	}

	// Returns the polarity of the slot where this connector is connected.
	public int GetPolarity() {
		if (meshRenderer.material.name.Contains(negativeMaterial.name)) {
			return 1;
		} else if (meshRenderer.material.name.Contains(positiveMaterial.name)) {
			return -1;
		} else {
			return 0;
		}
	}
}
