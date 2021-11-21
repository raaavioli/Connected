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
    [Range(0.0f, 1.0f)]
    float Intensity = 0.0f;
    [SerializeField]
    [Range(1.0f, 30.0f)]
    float MaxIntensity = 30.0f;

    Renderer FilamentRenderer;
    MaterialPropertyBlock FilamentMpb;

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
        LightSource.intensity = MaxIntensity * Intensity;
    }

    public void SetIntensity(float intensity) {
        if (intensity > 1f)
            intensity = 1;
        if (intensity < 0)
            intensity = 0;
        Intensity = intensity;
    }
}
