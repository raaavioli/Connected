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

        TextMesh amountText;
        public int maxTools = 5;
        public int currentTools = 0;

        private void Awake()
        {
            markerRenderer = GetComponent<Renderer>();

            Mpb = new MaterialPropertyBlock();
            Mpb.SetColor("_Color", Colors.DarkBrown);

            amountText = GetComponentInChildren<TextMesh>();
            
            if (toolPrefab == null)
                amountText.text = "";
        }

        private void Start()
        {
            ToolManager.Add(this);
            if (CanInstantiate())
            {
                GameObject tool = InstantiateTool(transform.position);
                tool.transform.localScale = new Vector3(2, 2, 1); // Due to now inheriting the scale from its parent (the tool markers), this has to compensate for that so the tools are the proper size.
                tool.transform.position -= transform.forward * 0.1f;
                Collider[] colliders = tool.GetComponentsInChildren<Collider>();
                Destroy(tool.GetComponent<Throwable>());
                Destroy(tool.GetComponent<Interactable>());
                Destroy(tool.GetComponent<Rigidbody>());
                foreach (Collider collider in colliders) {
                    Destroy(collider);
				}
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
                return Instantiate(toolPrefab, position, Quaternion.Euler(0,0,0), transform);
            }
            else
            {
                Debug.LogError("Cannot instantiate null prefab in ToolMarker");
                return null;
            }
        }

        public void ToolDeleted() 
        {
            currentTools -= 1;
            updateAmountText();
        }

        private void updateAmountText(){
            amountText.text = string.Format("{0}/{1}", (maxTools-currentTools), maxTools);
        }

        // STEAM VR FUNCTIONS

        public void SpawnAndAttach(Hand hand)
        {
            if (currentTools < maxTools){
                GameObject prefabObject = Instantiate(toolPrefab);
                hand.AttachObject(prefabObject, GrabTypes.Pinch);

                prefabObject.GetComponent<SpawnAmount>().spawner = (this);

                currentTools += 1;
                updateAmountText();
            }
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