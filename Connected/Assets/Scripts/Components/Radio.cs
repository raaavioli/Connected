using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Radio : GeneralComponent
{
    private const float MAX_POWER = 12f;
    private const float MIN_POWER = 6f;

    private const float MAX_VOLUME = 10f;
    private const float MIN_FREQ = 88.1f;
    private const float MAX_FREQ = 108.1f;

    private AudioSource audioSource;
    private uint playingIndex;

    [SerializeField]
    [Range(0, MAX_VOLUME)]
    private float volume = MAX_VOLUME;
    [SerializeField]
    private AudioClip[] channelSounds;
    [SerializeField]
    [Range(MIN_FREQ, MAX_FREQ)]
    private float channelFrequency;

    [SerializeField]
    private GameObject channelSlider;
    [SerializeField]
    private GameObject channelWheel;
    [SerializeField]
    private GameObject volumeWheel;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        resistance = 5f;
        channelFrequency = MIN_FREQ;
        PlayCurrentChannel();
    }

    void Update()
    {
        if (CalculatePower() < MIN_POWER)
        {
            StopPlaying();
        }
        else
        {
            if (!audioSource.isPlaying || playingIndex != GetChannelIndex())
                PlayCurrentChannel();
            float volumeAngle = -270f * volume / MAX_VOLUME;
            volumeWheel.transform.eulerAngles = new Vector3(0, 0, volumeAngle);
            float freq01 = (channelFrequency - MIN_FREQ) / (MAX_FREQ - MIN_FREQ);
            channelWheel.transform.eulerAngles = new Vector3(0, 0, freq01 * -270f);
            Vector3 position = channelSlider.transform.position;
            position.x = -0.1f + freq01 * 0.2f;
            channelSlider.transform.position = position;

            audioSource.volume = volume / MAX_VOLUME;
        }
    }

    private uint GetChannelIndex()
    {
        return (uint) (channelSounds.Length * (channelFrequency - MIN_FREQ) / (MAX_FREQ - MIN_FREQ));
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
