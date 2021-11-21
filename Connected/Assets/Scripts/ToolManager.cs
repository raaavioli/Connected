using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    private List<ToolMarker> toolMarkers;
    private static ToolManager instance;

    private void Awake()
    {
        if (instance == null) 
            instance = this;
        else 
            Destroy(this);

        toolMarkers = new List<ToolMarker>();
    }

    public static void Add(ToolMarker toolMarker)
    {
        instance.toolMarkers.Add(toolMarker);
    }

    public static void Remove(ToolMarker toolMarker)
    {
        instance.toolMarkers.Remove(toolMarker);
    }
}
