using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collider) {
		var mover = collider.GetComponentInParent<Mover>();
		if (mover != null) Destroy(mover.gameObject);
	}

}
