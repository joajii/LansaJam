using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {

	[SerializeField] float goalDelay;
	[SerializeField] GameObject model;

	Coroutine winRoutine;

	private void OnTriggerEnter2D(Collider2D other) {
		if (PlayerController.Instance.Recording || winRoutine != null) return;
		winRoutine = StartCoroutine(WinRoutine(goalDelay));
	}

	IEnumerator WinRoutine(float delay) {
		model.SetActive(false);
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
