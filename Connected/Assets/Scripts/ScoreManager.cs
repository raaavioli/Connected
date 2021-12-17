using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour {

	[SerializeField]
	private TextMesh scoreText;

	private static ScoreManager instance;
	private int score = 0;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this);
		}

		instance.UpdateScore();
	}

	public static void AddScore(int score) {
		if (instance != null) {
			instance.score += score;
			instance.UpdateScore();
		}
	}

	private void UpdateScore() {
		scoreText.text = score.ToString();
	}
}
