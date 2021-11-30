using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class WireSpawner : MonoBehaviour
{
    [SerializeField]
    private SteamVR_Action_Boolean SpawnWire;
    [SerializeField]
    private GameObject wirePrefab;
    [SerializeField]
    private Player player;

    void Start()
    {
        SpawnWire.AddOnStateDownListener(TriggersDown, SteamVR_Input_Sources.Any);
    }

    public void TriggersDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        bool leftHandFree = player.leftHand.currentAttachedObject == null;
        bool rightHandFree = player.rightHand.currentAttachedObject == null;

        if (leftHandFree && rightHandFree)
        {
            GameObject wire = Instantiate(wirePrefab);
            GameObject startConnector = wire.transform.GetChild(0).gameObject;
            GameObject endConnector = wire.transform.GetChild(1).gameObject;

            player.leftHand.AttachObject(startConnector, GrabTypes.Pinch);
            player.rightHand.AttachObject(endConnector, GrabTypes.Pinch);
        }
    }
}