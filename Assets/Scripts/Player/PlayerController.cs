using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	[SerializeField] Mover player;
	[SerializeField] Mover recordPlayer;
	[SerializeField] Rigidbody2D ghost;
	[Space]
	[SerializeField] KeyCode[] left = { KeyCode.LeftArrow, KeyCode.A };
	[SerializeField] KeyCode[] right = { KeyCode.RightArrow, KeyCode.D };
	[SerializeField] KeyCode[] jump = { KeyCode.UpArrow, KeyCode.A };
	[SerializeField] KeyCode[] record = { KeyCode.Space };

	public static PlayerController Instance { get; protected set; }

	public bool Recording { get; protected set; }
	bool playing;

	Mover target;
	List<Vector2> positions = new List<Vector2>();
	int positionIndex = 0;

	private void Awake() {
		target = player;
		Instance = this;
	}

	private void Start() {
		CameraManager.Instance.SnapToTarget(target);
	}

	void Update() {
		int press = 0;
		bool tryJump = GetKeyDown(jump);
		bool tryRecord = GetKeyDown(record);

		if (GetKey(left)) press -= 1;
		if (GetKey(right)) press += 1;

		target.SetMoveState(press, tryJump);

		if (tryRecord) {
			if (playing) StopPlaying();
			else if (Recording) StopRecord();
			else StartCoroutine(StartRecord());
		}

		if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void FixedUpdate() {
		if (Recording) {
			positions.Add(target.Rigidbody.position);
		} else if (playing) {
			ghost.position = positions[positionIndex];
			positionIndex = (positionIndex + 1) % positions.Count;
		}

		CameraManager.Instance.MoveToTarget(target);
	}

	private IEnumerator StartRecord() {
		if (!player.IsGrounded()) yield break;
		yield return new WaitForFixedUpdate();
		positions.Clear();
		target = recordPlayer;
		recordPlayer.gameObject.SetActive(true);
		recordPlayer.Rigidbody.position = player.Rigidbody.position;
		recordPlayer.transform.position = player.transform.position;
		recordPlayer.VSpeed = player.VSpeed;
		Recording = true;
	}

	private void StopRecord() {
		positionIndex = 0;
		ghost.transform.position = positions[0];
		ghost.gameObject.SetActive(true);
		recordPlayer.gameObject.SetActive(false);
		target = player;
		Recording = false;
		playing = true;
	}

	private void StopPlaying() {
		ghost.gameObject.SetActive(false);
		playing = false;
	}

	private bool GetKey(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKey(key))
				return true;
		}
		return false;
	}

	private bool GetKeyDown(params KeyCode[] keys) {
		foreach (KeyCode key in keys) {
			if (Input.GetKeyDown(key))
				return true;
		}
		return false;
	}
}
