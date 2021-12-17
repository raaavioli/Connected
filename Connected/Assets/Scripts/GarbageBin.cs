using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    [SerializeField]
    AudioClip[] destroySounds;
    [SerializeField]
    private AudioSource audioSource;

    private void OnTriggerEnter(Collider collider)
    {
        Destroy(collider.gameObject);
        PlaySound();
    }

    private void PlaySound() {
		audioSource.clip = destroySounds[Random.Range(0,destroySounds.Length)];
        audioSource.Play();
    }
}
