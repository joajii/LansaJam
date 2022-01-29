using UnityEngine;

public class CameraManager : MonoBehaviour {
	
	public static CameraManager Instance { get; protected set; }

	public Vector3 Position { get { return transform.position; } }

	[SerializeField] Vector3 offset;
	[SerializeField] float yLeniency;
	[SerializeField] float fastSpeed;
	[SerializeField] float slowSpeed;

	void Awake() {
		Instance = this;
	}

	public void MoveToTarget(Mover mover) {
		var x = mover.transform.position.x + offset.x;
		var speed = slowSpeed;

		var y = mover.transform.position.y + offset.y;
		if (!mover.IsGrounded()) {
			if (Mathf.Abs(y - transform.position.y) < yLeniency) y = transform.position.y;
			else {
				speed = fastSpeed;
				y += (y > transform.position.y) ? -yLeniency : yLeniency;
			}
		}

		transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, y, mover.transform.position.z + offset.z), speed * Time.fixedDeltaTime);
		
	}

	public void SnapToTarget(Mover mover) {
		transform.position = mover.transform.position + offset;
	}
}
