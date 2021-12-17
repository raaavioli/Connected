using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class SceneChanger : MonoBehaviour {

	private static SceneChanger instance;

	[SerializeField]
	private SteamVR_Action_Boolean LeftGripPressed;
	[SerializeField]
	private SteamVR_Action_Boolean RightGripPressed;

	private bool leftGripped = false;
	private bool rightGripped = false;
	private bool justChanged = false;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this);

			LeftGripPressed.AddOnChangeListener(LeftGripGrab, SteamVR_Input_Sources.LeftHand);
			RightGripPressed.AddOnChangeListener(RightGripGrab, SteamVR_Input_Sources.RightHand);
		} else {
			Destroy(this);
		}
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space) || ((leftGripped && rightGripped) && !justChanged)) {
			if (SceneManager.GetActiveScene().name == "RoomScene") {
				CleanUp();
				justChanged = true;
				SceneManager.LoadScene("BestScene");
			} else {
				CleanUp();
				justChanged = true;
				SceneManager.LoadScene("RoomScene");
			}
		} else if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}

		if (!(leftGripped && rightGripped) && justChanged) {
			justChanged = false;
		}
	}

	private void CleanUp() {
		Destroy(GameObject.Find("Player"));
		GameObject[] basePlates = GameObject.FindGameObjectsWithTag("BasePlate");
		foreach (GameObject basePlate in basePlates) {
			Destroy(basePlate);
		}
	}

	private void LeftGripGrab(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newValue) {
		leftGripped = newValue;
	}

	private void RightGripGrab(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newValue) {
		rightGripped = newValue;
	}
}
