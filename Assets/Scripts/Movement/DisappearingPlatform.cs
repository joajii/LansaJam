using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour {

	[SerializeField] float disappearDuration;
	[SerializeField] float disappearDelay;

	[SerializeField] GameObject platform;

	Coroutine disappearRoutine;

	void  OnTriggerEnter2D(Collider2D collider) {
		if (disappearRoutine == null)
			disappearRoutine = StartCoroutine(DisappearRoutine(disappearDelay, disappearDuration));
	}

	IEnumerator DisappearRoutine(float delay, float duration) {
		yield return new WaitForSeconds(delay);
		platform.SetActive(false);
		yield return new WaitForSeconds(duration);
		bool free = false;
		while (true) {
			foreach(BoxCollider2D box in platform.GetComponentsInChildren<BoxCollider2D>()) {
				if (Physics2D.OverlapBoxAll(platform.transform.position, box.size, 0, LayerMask.GetMask("Player")).Length == 0) {
					free = true;
				} else {
					free = false;
					break;
				}
			}
			if (free) break;
			yield return null;
		}
		platform.SetActive(true);
		disappearRoutine = null;
	}

}
