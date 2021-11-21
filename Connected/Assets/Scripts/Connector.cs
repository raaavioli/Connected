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

	private void Awake() {
		meshRenderer = GetComponent<MeshRenderer>();
		rb = GetComponent<Rigidbody>();
		trigger = GetComponent<SphereCollider>();
		interactable = GetComponent<Interactable>();
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

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Slot")) {
            connectedSlot = other.GetComponent<Slot>();
			if (IsHeld()) {
				interactable.attachedToHand.DetachObject(this.gameObject);
			}
			ConnectionActions();
		}
	}

	private void ConnectionActions() {
		if (connectedSlot.InitiateConnection(this)) {
			bool positive = connectedSlot.positive;
			rb.isKinematic = true;
			transform.parent = connectedSlot.transform;
			trigger.enabled = false;

			meshRenderer.material = positive ? positiveMaterial : negativeMaterial;
			associatedWire.RecolorWire(this, positive ? positiveColor : negativeColor);
		}
	}

	public void DisconnectionActions() {
		connectedSlot.Disconnect();
		connectedSlot = null;
		rb.isKinematic = false;
		transform.parent = associatedWire.transform;
		StartCoroutine(DelayEnableTrigger());

		if (!IsHeld()) {
			rb.AddForce(Vector3.up, ForceMode.VelocityChange);
		}

		meshRenderer.material = neutralMaterial;
		associatedWire.RecolorWire(this, neutralColor);
	}

	private bool IsHeld() {
		return interactable != null && interactable.attachedToHand != null;
	}

	private IEnumerator DelayEnableTrigger() {
		yield return new WaitForSeconds(1.0f);
		trigger.enabled = true;
	}
}
