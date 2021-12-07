using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class WireSpawner : MonoBehaviour
{
    [SerializeField]
    private SteamVR_Action_Single LeftHandPressed;
    [SerializeField]
    private SteamVR_Action_Single RightHandPressed;
    [SerializeField]
    private GameObject wirePrefab;
    [SerializeField]
    private Player player;

    private bool leftHandPressed;
    private bool rightHandPressed;

    void Start()
    {
        LeftHandPressed.AddOnAxisListener(LeftTriggerSqueeze, SteamVR_Input_Sources.LeftHand);

        RightHandPressed.AddOnAxisListener(RightTriggerSqueeze, SteamVR_Input_Sources.RightHand);
    }

    private void Update()
	{
        if (player == null)
        {
            Debug.LogWarning("WireSpawner needs a non-null reference to the player in the scene.");
		}

        bool leftHandFree = player.leftHand.currentAttachedObject == null;
        bool rightHandFree = player.rightHand.currentAttachedObject == null;

        if (leftHandFree && rightHandFree && leftHandPressed && rightHandPressed)
        {
            GameObject wire = Instantiate(wirePrefab);
            GameObject startConnector = wire.transform.GetChild(0).gameObject;
            startConnector.transform.rotation = Quaternion.Inverse(startConnector.transform.rotation);
            GameObject endConnector = wire.transform.GetChild(1).gameObject;
            endConnector.transform.rotation = Quaternion.Inverse(endConnector.transform.rotation);

            player.leftHand.AttachObject(startConnector, GrabTypes.Pinch);
            player.rightHand.AttachObject(endConnector, GrabTypes.Pinch);
        }
    }

    public void LeftTriggerSqueeze(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {
        leftHandPressed = newAxis > 0.999f;
    }

    public void RightTriggerSqueeze(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
    {
        rightHandPressed = newAxis > 0.999f;
    }
}