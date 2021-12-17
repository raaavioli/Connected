using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageBin : MonoBehaviour
{
    [SerializeField]
    AudioClip[] destroySounds;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private int score = 1;

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Slot")) {
            Destroy(collider.gameObject);
            PlaySound();
            ScoreManager.AddScore(score * (collider.gameObject.name.Contains("Radio") ? 2 : 1));
        }
        
    }

    private void PlaySound() {
		audioSource.clip = destroySounds[Random.Range(0,destroySounds.Length)];
        audioSource.Play();
    }
}
