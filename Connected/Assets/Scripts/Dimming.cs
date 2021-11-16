using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimming : MonoBehaviour
{
    MaterialPropertyBlock FilamentMpb;

    [SerializeField]
    GameObject Filament;

    [SerializeField]
    Light LightSource;

    [SerializeField]
    [Range(0.0f, 20.0f)]
    float Intensity = 1.0f;
    float MaxIntensity = 20.0f;

    private Color filamentColor = new Color(0.7490196f, 0.427451f, 0);

    private void Start()
    {
        FilamentMpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        FilamentMpb.SetColor("_EmissionColor", filamentColor * Intensity);
        Filament.GetComponent<Renderer>().SetPropertyBlock(FilamentMpb);
        LightSource.intensity = Intensity / MaxIntensity;
    }
}
