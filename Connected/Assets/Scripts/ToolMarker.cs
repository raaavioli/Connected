using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
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
                tool.transform.localScale = new Vector3(1, 1, 1);
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
                return Instantiate(toolPrefab, position, transform.rotation);
            }
            else
            {
                Debug.LogError("Cannot instantiate null prefab in ToolMarker");
                return null;
            }
        }

        // STEAM VR FUNCTIONS

        public void SpawnAndAttach(Hand hand)
        {
            GameObject prefabObject = Instantiate(toolPrefab);
            hand.AttachObject(prefabObject, GrabTypes.Scripted);
        }

        // This is like the update function. It is polled whenever a steamVR "hand" hovers over it.
		private void HandHoverUpdate(Hand hand)
		{
            GrabTypes startingGrab = hand.GetGrabStarting();

            if (startingGrab != GrabTypes.None)
            {
                SpawnAndAttach(hand);
            }
		}
    }
}