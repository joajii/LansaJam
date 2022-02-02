using UnityEngine;

public class Mover : MonoBehaviour {

	[SerializeField] float walkSpeed;
	[SerializeField] float jumpSpeed;
	[SerializeField] float gravity;
	[SerializeField] float maxVSpeed;
	[SerializeField] float minVSpeed;
	[SerializeField] float coyoteTime;
	[SerializeField] float jumpBuffer;

	int jumpMask = -1;
	int groundMask = -1;

	int move = -1;
	bool jump;

	float jumpInputTime = float.MinValue;
	float lastGroundedTime = float.MinValue;

	static float epsilon = 0.01f;

	public Rigidbody2D Rigidbody { get; protected set; }
	new BoxCollider2D collider;

	public float VSpeed { get; set; }
	public float HSpeed { get; set; }

	private void Awake() {
		groundMask = LayerMask.GetMask("Solid");
		jumpMask = LayerMask.GetMask("Ghost");
		Rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
	}

	public void SetMoveState(int move, bool jump) {
		if (move != 0) this.move = move;
		if (jump != false) {
			this.jump = true;
			jumpInputTime = Time.time;
		}
	}

	void FixedUpdate() {
		if (!IsGrounded()) VSpeed -= gravity * Time.fixedDeltaTime;
		else {
			lastGroundedTime = Time.time;
			if (VSpeed < 0) VSpeed = 0;
		}
		
		if (jump && CanJump()) {
			VSpeed = jumpSpeed;
			jump = false;
		}
		VSpeed = Mathf.Clamp(minVSpeed, VSpeed, maxVSpeed);

		HSpeed = move * walkSpeed;

		Move(HSpeed * Time.fixedDeltaTime, VSpeed * Time.fixedDeltaTime);

		move = 0;
		jump = jump && Time.time - jumpInputTime <= jumpBuffer;
	}

	private void Move(float x, float y) {

		var pos = Unstuck(Rigidbody.position);

		if (x != 0) {
			var moveCheck = Physics2D.BoxCast(pos, collider.size, 0, Vector2.right, x + epsilon, groundMask);
			if (moveCheck.collider != null) {
				x = Mathf.Sign(x) * Mathf.Max(0, moveCheck.distance - epsilon);
				HSpeed = 0;
			}
		}

		if (y != 0) {
			var moveCheck = Physics2D.BoxCast(pos + Vector2.right * x, collider.size, 0, Vector2.up, y + epsilon, groundMask);
			if (moveCheck.collider != null) {
				y = Mathf.Sign(y) * Mathf.Max(0, moveCheck.distance - epsilon);
				VSpeed = 0;
			}
		}

		Rigidbody.MovePosition(pos + Vector2.right * x + Vector2.up * y);
	}

	private Vector2 Unstuck(Vector2 start) {
		var overlap = Physics2D.OverlapBoxAll(start, collider.size * (1 - epsilon), 0, groundMask);

		if (overlap.Length == 0) return start;

		foreach (Collider2D coll in overlap) {
			var dir = Mathf.Sign(coll.transform.position.x - start.x);
			var stuckCheck = Physics2D.Raycast(start - Vector2.right * dir * collider.size.x/2f, Vector2.right * dir, collider.size.x, groundMask);
			if (stuckCheck.collider != null) {
				return stuckCheck.point - Vector2.right * dir * (collider.size.x/2f + epsilon);
			}


			dir = Mathf.Sign(coll.transform.position.y - start.y);
			stuckCheck = Physics2D.Raycast(start - Vector2.up * dir * collider.size.y / 2f, Vector2.up * dir, collider.size.y, groundMask);
			if (stuckCheck.collider != null)  {
				return stuckCheck.point - Vector2.up * dir * (collider.size.y/2f + epsilon);
			}
		}

		return start;
	}

	public bool IsGrounded() {
		var groundCheck = Physics2D.BoxCast(Rigidbody.position, collider.size, 0, Vector2.down, epsilon, groundMask);
		return groundCheck.collider != null;
	}

	private bool CanJump() {
		var onGhost = Physics2D.OverlapBoxAll(Rigidbody.position, collider.size, 0, jumpMask).Length != 0;
		return onGhost || (Time.time - lastGroundedTime <= coyoteTime);
	}
}
