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
    [SerializeField]
    private AudioClip[] sounds;

    private bool leftHandPressed;
    private bool rightHandPressed;
    private AudioSource audioSource;

    private int maxWires = 15;
    public int currentWires = 0;
    private bool justSpawned = false;

    void Start()
    {
        LeftHandPressed.AddOnAxisListener(LeftTriggerSqueeze, SteamVR_Input_Sources.LeftHand);

        RightHandPressed.AddOnAxisListener(RightTriggerSqueeze, SteamVR_Input_Sources.RightHand);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
	{
        if (player == null)
        {
            Debug.LogWarning("WireSpawner needs a non-null reference to the player in the scene.");
            return;
		} else if (player.leftHand == null || player.rightHand == null) {
            Debug.LogWarning("No left or right hands were found.");
            return;
		}

        bool leftHandFree = player.leftHand.currentAttachedObject == null;
        bool rightHandFree = player.rightHand.currentAttachedObject == null;

        if (leftHandFree && rightHandFree && leftHandPressed && rightHandPressed && currentWires < maxWires && !justSpawned)
        {
            justSpawned = true;
            GameObject wire = Instantiate(wirePrefab);

            wire.GetComponent<Wire>().spawner = this;

            GameObject startConnector = wire.transform.GetChild(0).gameObject;
            GameObject endConnector = wire.transform.GetChild(1).gameObject;

            Hand.AttachmentFlags flags = startConnector.GetComponent<Throwable>().attachmentFlags;

            player.leftHand.AttachObject(startConnector, GrabTypes.Pinch, flags, wire.transform);
            player.rightHand.AttachObject(endConnector, GrabTypes.Pinch, flags, wire.transform);

            currentWires += 1;
        }

        if (!(leftHandPressed && rightHandPressed) && justSpawned) {
            justSpawned = false;
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

    private void PlaySound()
    {
        audioSource.clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        audioSource.Play();
    }
}