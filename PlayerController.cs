using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	[System.Serializable]
	public class MovementSettings
	{
		public float speed = 4.0f;
		public float jumpSpeed = 5.0f;
		public float gravity = 15.0f;
		public Vector3 spawnBuffer = new Vector3 (0, 0, 2.0f);
	}

	[System.Serializable]
	public class MovementInput
	{
		public string MOVE_HORIZONTAL = "Horizontal";
		public string MOVE_VERTICAL = "Vertical";
		public string JUMP = "Jump";
	}

	private bool jumping = false;
	private float jump;
	private Vector3 moveDirection;
	private float horizontal;
	private float vertical;
	private Vector3 forward;
	private Vector3 right;


	private CharacterController player;
	private Animator animator;
	public SceneTransition transition = new SceneTransition ();
	public MovementSettings movement = new MovementSettings ();
	public MovementInput input = new MovementInput ();

	void Start()
	{
		player = GetComponent<CharacterController> ();
		animator = GetComponent<Animator> ();
		animator.SetBool ("start", false);
		if (transition.CheckScene(transition.OVERWORLD_SCENE)) 
		{
			player.transform.position = GameManager.Instance.spawn;
		}

		if (GameManager.Instance.start) 
		{
			animator.Play ("Get_Up");
		}

	}

	void Update()
	{
		GetInput ();
		MovePlayer ();
		//Jump ();
		GameManager.Instance.ToggleInvert();
		GameManager.Instance.CursorLock ();

		if (Input.GetKeyDown (KeyCode.Escape) && !GameManager.Instance.paused) 
		{
			GameManager.Instance.PauseGame();
		}

		moveDirection.y -= movement.gravity * Time.deltaTime;
		player.Move(moveDirection * movement.speed * Time.deltaTime);

		if (moveDirection.x != 0 || moveDirection.z != 0) 
		{
			animator.SetBool ("isWalking", true);
		} 
		else 
		{
			animator.SetBool ("isWalking", false);
		}

		if (moveDirection.y > 0) {
			animator.SetBool ("isJumping", true);
		} 
		else 
		{
			animator.SetBool ("isJumping", false);
		}

	}
		
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.CompareTag(transition.TO_MEMORY1))
		{
			GameManager.Instance.spawn = player.transform.position + movement.spawnBuffer;
			transition.TransitionScene (transition.MEMORY1_SCENE);
		}

		if (col.gameObject.CompareTag(transition.TO_OVERWORLD))
		{
			transition.TransitionScene (transition.OVERWORLD_SCENE);
		}
	}

	void MovePlayer()
	{
		if (player.isGrounded) 
		{
			jumping = false;

			forward = Camera.main.transform.TransformDirection (Vector3.forward);
			forward.y = 0f;
			forward = forward.normalized;
			right = new Vector3(forward.z, 0f, -forward.x);
			moveDirection = (horizontal * right + vertical * forward);

			if (moveDirection != Vector3.zero)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), 10f * Time.smoothDeltaTime);
				transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
			}

			if (moveDirection.sqrMagnitude > 1f) 
			{
				moveDirection = moveDirection.normalized;
			}
				
		}
	}

	void Jump()
	{
		if (jump > 0 && !jumping) 
		{
			moveDirection.y = movement.jumpSpeed;
			jumping = true;
		} 
	}

	void GetInput()
	{
		if (!GameManager.Instance.start) 
		{
			horizontal = Input.GetAxis (input.MOVE_HORIZONTAL);
			vertical = Input.GetAxis (input.MOVE_VERTICAL);
			jump = Input.GetAxis (input.JUMP);
		}
	}

	void doneGetUp()
	{
		animator.SetBool ("start", true);
		GameManager.Instance.start = false;
	}
		
}

