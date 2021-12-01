using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class TunerWheel : MonoBehaviour
{
    private Interactable interactable;

    private Hand tuningHand;
    private float startHandRotation;
    private float startWheelRotation;

    public float GetValue01()
    {
        // Wheel rotation angle is -270 to 0, or simply 90 to 360.
        return Mathf.Clamp01((360f - transform.eulerAngles.z) / 270f);
    }

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.onAttachedToHand += OnAttachedToHand;
        interactable.onDetachedFromHand += OnDetachFromHand;
    }
    private void OnDestroy()
    {
        interactable.onAttachedToHand -= OnAttachedToHand;
        interactable.onDetachedFromHand -= OnDetachFromHand;
    }

    void Update()
    {
        if (tuningHand != null)
        {
            float currentHandRotation = tuningHand.transform.localEulerAngles.z;
            float deltaRot = currentHandRotation - startHandRotation;

            // Wheel rotation angle is -270 to 0, or simply 90 to 360.
            float currentRot = Mathf.Clamp(startWheelRotation + deltaRot, -270f, 0f);
            transform.eulerAngles = new Vector3(0, 0, currentRot);
        }
    }

    private void OnAttachedToHand(Hand hand)
    {
        tuningHand = hand;
        startHandRotation = tuningHand.transform.localEulerAngles.z;
        startWheelRotation = tuningHand.transform.eulerAngles.z;
    }

    private void OnDetachFromHand(Hand hand)
    {
        tuningHand = null;
    }
}
