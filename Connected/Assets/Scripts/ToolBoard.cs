using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBoard : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    int Rows;
    [SerializeField]
    [Range(1, 10)]
    int Columns;
    [SerializeField]
    private GameObject ToolMarkerPrefab;
    [SerializeField]
    private GameObject[] ToolObjectPrefabs;

    private List<Tool> Tools = new List<Tool>();

    void Start()
    {


        for (int i = 0; i < ToolObjectPrefabs.Length; i++)
        {
            GameObject marker = Instantiate(ToolMarkerPrefab, gameObject.transform);
            float y = -0.45f * (i / Columns);
            float x = -0.45f * (i % Columns);
            marker.transform.position += new Vector3(x, y, 0);
            marker.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            GameObject toolObject = null;
            if (ToolObjectPrefabs[i])
            {
                toolObject = Instantiate(ToolObjectPrefabs[i], marker.transform);
                toolObject.transform.position += new Vector3(0, 0, 0.10f);
                toolObject.transform.localScale *= 1 / 0.04f;
            }   
            Tools.Add(new Tool(toolObject, marker));
        }
    }

    private void Update()
    {
        foreach (Tool tool in Tools)
        {
            tool.Update();
        }
    }

    
}

public class Tool
{
    private GameObject toolPrefab;
    private GameObject marker;

    MaterialPropertyBlock InactiveMPB;
    MaterialPropertyBlock ActiveMPB;

    List<GameObject> toolInstances = new List<GameObject>();

    public Tool(GameObject toolPrefab, GameObject marker)
    {
        this.toolPrefab = toolPrefab;
        this.marker = marker;

        InactiveMPB = new MaterialPropertyBlock();
        InactiveMPB.SetColor("_Color", Colors.DarkBrown);
        ActiveMPB = new MaterialPropertyBlock();
        ActiveMPB.SetColor("_Color", Colors.Orange);
    }

    public void Update()
    {

        if (toolPrefab != null)
            marker.GetComponent<Renderer>().SetPropertyBlock(ActiveMPB);
        else
            marker.GetComponent<Renderer>().SetPropertyBlock(InactiveMPB);

    }
}


