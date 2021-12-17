using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class SpawnAmount : MonoBehaviour
    {
        [HideInInspector]
        public ToolMarker spawner;

        private void OnDestroy() {
            if (spawner != null)
                spawner.ToolDeleted();
        }
    }
}