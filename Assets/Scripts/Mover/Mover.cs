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

	public Rigidbody2D Rigidbody { get; protected set; }

	public float Vspeed { get; set; }

	private void Awake() {
		groundMask = LayerMask.GetMask("Solid");
		jumpMask = LayerMask.GetMask("Solid", "Ghost");
		Rigidbody = GetComponent<Rigidbody2D>();
	}
	
	public void SetMoveState(int move, bool jump) {
		if (move != 0) this.move = move;
		if (jump != false) this.jump = jump;
	}

	void FixedUpdate() {
		if (!IsGrounded()) Vspeed -= gravity * Time.fixedDeltaTime;
		else Vspeed = 0;
		if (jump && CanJump()) Vspeed = jumpSpeed;
		Vspeed = Mathf.Clamp(minVSpeed, Vspeed, maxVSpeed);

		Rigidbody.MovePosition(Rigidbody.position + (Vector2.up * Vspeed + Vector2.right * move * walkSpeed) * Time.fixedDeltaTime);
		move = 0;
		jump = false;
	}

	private bool IsGrounded() {
		var groundCheck = Physics2D.Raycast(Rigidbody.position + Vector2.down * transform.lossyScale.y / 2f, Vector2.down, 0.01f, groundMask);
		return groundCheck.collider != null;
	}

	private bool CanJump() {
		var groundCheck = Physics2D.Raycast(Rigidbody.position + Vector2.up * transform.lossyScale.y / 2f, Vector2.down, 0.01f + transform.lossyScale.y, jumpMask);
		return groundCheck.collider != null;
	}
}
