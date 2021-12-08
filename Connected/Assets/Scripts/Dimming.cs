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
    const float FilamentIntensity = 2.0f;

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
        int lightOn = (Intensity > 0) ? 1 : 0;
        FilamentMpb.SetColor("_EmissionColor", lightOn * Colors.FilamentColor * (1 + FilamentIntensity * Intensity));
        FilamentRenderer.SetPropertyBlock(FilamentMpb);
        LightSource.intensity = 2f * Intensity;
    }

    public void SetIntensity(float intensity)
    {
        if (intensity > 1f)
            intensity = 1;
        if (intensity < 0)
            intensity = 0;
        Intensity = intensity;
    }
}
