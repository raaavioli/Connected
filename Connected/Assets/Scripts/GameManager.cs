using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private static GameManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public static Player GetPlayer() {
        return instance.player;
    }
}
