using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radio : GeneralComponent
{
    private const float MAX_POWER = 12f;
    private const float MIN_POWER = 6f;

    private AudioSource audioSource;
    private uint playingIndex;

    [SerializeField]
    [Range(0, 1)]
    private float volume = 1;
    [SerializeField]
    private AudioClip[] channelSounds;

    [SerializeField]
    private GameObject channelSlider;
    [SerializeField]
    private TunerWheel channelWheel;
    [SerializeField]
    private TunerWheel volumeWheel;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        resistance = 5f;
        PlayCurrentChannel();
    }

    void Update()
    {
        volume = volumeWheel.GetValue01();
        audioSource.volume = volume;

        Vector3 position = channelSlider.transform.localPosition;
        position.x = -0.1f + channelWheel.GetValue01() * 0.2f;
        channelSlider.transform.localPosition = position;

        if (CalculatePower() < MIN_POWER)
        {
            StopPlaying();
        }
        else if (!audioSource.isPlaying || playingIndex != GetChannelIndex())
        {
            PlayCurrentChannel();
        }
    }

    private uint GetChannelIndex()
    {
        float value = channelWheel.GetValue01();
        // if value = 1 exactly, then value * channelSounds.Length is out of bounds.
        if (value == 1)
            value -= float.Epsilon;
        return (uint)(value * channelSounds.Length);
    }

    private void PlayCurrentChannel()
    {
        uint channelIndex = GetChannelIndex();
        if (channelIndex < channelSounds.Length)
        {
            StopPlaying();
            audioSource.clip = channelSounds[channelIndex];
            audioSource.Play();
            playingIndex = channelIndex;
        }
    }

    private void StopPlaying()
    {
        audioSource.Stop();
    }
}
