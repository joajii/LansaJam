using UnityEngine;

public class Mover : MonoBehaviour {

	[SerializeField] float walkSpeed;
	[SerializeField] float jumpSpeed;
	[SerializeField] float gravity;
	[SerializeField] float maxVSpeed;
	[SerializeField] float minVSpeed;

	int jumpMask = -1;
	int groundMask = -1;

	int move = -1;
	bool jump;

	float lastXHitTime;
	float lastYHitTime;
	float lastGroundedTime;

	static float epsilon = 0.01f;

	public Rigidbody2D Rigidbody { get; protected set; }
	new BoxCollider2D collider;

	public float VSpeed { get; set; }
	public float HSpeed { get; set; }

	private void Awake() {
		groundMask = LayerMask.GetMask("Solid");
		jumpMask = LayerMask.GetMask("Solid", "Ghost");
		Rigidbody = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
	}

	public void SetMoveState(int move, bool jump) {
		if (move != 0) this.move = move;
		if (jump != false) this.jump = jump;
	}

	void FixedUpdate() {
		if (!IsGrounded()) VSpeed -= gravity * Time.fixedDeltaTime;
		else VSpeed = 0;
		if (jump && CanJump()) VSpeed = jumpSpeed;
		VSpeed = Mathf.Clamp(minVSpeed, VSpeed, maxVSpeed);

		HSpeed = move * walkSpeed;

		Move(HSpeed * Time.fixedDeltaTime, VSpeed * Time.fixedDeltaTime);

		move = 0;
		jump = false;
	}

	private void Move(float x, float y) {
		if (x != 0) {
			var moveCheck = Physics2D.BoxCast(Rigidbody.position, collider.size, 0, Vector2.right, x + epsilon, groundMask);
			if (moveCheck.collider == null) {
				lastXHitTime = -1;
			} else {
				x = Mathf.Sign(x) * Mathf.Max(0, moveCheck.distance - epsilon);
				if (lastXHitTime < 0) lastXHitTime = Time.time;
			}
		}

		if (y != 0) {
			var moveCheck = Physics2D.BoxCast(Rigidbody.position + Vector2.right * x, collider.size, 0, Vector2.up, y, groundMask);
			if (moveCheck.collider == null) {
				lastYHitTime = -1;
			} else {
				y = Mathf.Sign(y) * Mathf.Max(0, moveCheck.distance - epsilon);
				if (lastYHitTime < 0) lastYHitTime = Time.time;
			}
		}

		Rigidbody.MovePosition(Rigidbody.position + Vector2.right * x + Vector2.up * y);
	}

	private bool IsGrounded() {
		var groundCheck = Physics2D.BoxCast(Rigidbody.position, collider.size, 0, Vector2.down, epsilon, groundMask);
		return groundCheck.collider != null;
	}

	private bool CanJump() {
		var jumpCheck = Physics2D.BoxCast(Rigidbody.position, collider.size, 0, Vector2.down, epsilon, jumpMask);
		return jumpCheck.collider != null;
	}
}
