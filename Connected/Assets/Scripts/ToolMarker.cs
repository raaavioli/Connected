using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolMarker : MonoBehaviour
{
    [SerializeField]
    GameObject toolPrefab;

    Renderer markerRenderer;

    MaterialPropertyBlock Mpb;

    private void Awake()
    {
        markerRenderer = GetComponent<Renderer>();

        Mpb = new MaterialPropertyBlock();
        Mpb.SetColor("_Color", Colors.DarkBrown);
    }

    private void Start()
    {
        ToolManager.Add(this);
        if (CanInstantiate())
        {
            GameObject tool = InstantiateTool(transform.position);
            tool.transform.position -= transform.forward * 0.1f;
        }
    }

    private void OnDestroy()
    {
        ToolManager.Remove(this);
    }

    public void Update()
    {
        Mpb.SetColor("_Color", toolPrefab == null ? Colors.DarkBrown : Colors.Orange);
        Mpb.SetVector("_ObjectScale", transform.localScale);
        markerRenderer.SetPropertyBlock(Mpb, 0);
    }

    public bool CanInstantiate()
    {
        return toolPrefab != null;
    }

    public GameObject InstantiateTool(Vector3 position)
    {
        if (CanInstantiate())
        {
            return Instantiate(toolPrefab, position, transform.rotation * Quaternion.LookRotation(Vector3.back));
        }
        else
        {
            Debug.LogError("Cannot instantiate null prefab in ToolMarker");
            return null;
        }
    }
}
