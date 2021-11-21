using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimming : MonoBehaviour
{

    [SerializeField]
    GameObject Filament;
    [SerializeField]
    Light LightSource;
    [SerializeField]
    [Range(0.0f, 20.0f)]
    float Intensity = 0.0f;

    Renderer FilamentRenderer;
    MaterialPropertyBlock FilamentMpb;
    float MaxIntensity = 20.0f;

    private void Awake()
    {
        FilamentRenderer = Filament.GetComponent<Renderer>();
        FilamentMpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        FilamentMpb.SetColor("_EmissionColor", Colors.FilamentColor * Intensity);
        FilamentRenderer.SetPropertyBlock(FilamentMpb);
        LightSource.intensity = Intensity / MaxIntensity;
    }

    public void SetIntensity(float intensity) {
        Intensity = intensity;
    }
}
