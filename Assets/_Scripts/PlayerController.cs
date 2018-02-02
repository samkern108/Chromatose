using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;

public class PlayerController : MonoBehaviour, IRestartObserver {

	private bool inputDisabled = false;

	// movement config
	private float gravity = -30f;
	private float runSpeed = 7f;
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float inAirDamping = 5f;
	private float jumpHeight = 3f;

	private float normalizedHorizontalSpeed = 0;

	// A non-flipped PlayerTemp sprite faces right (1)
	public static int spriteFlipped = 1;

	private CharacterController2D _controller;
	private Animate _animate;
	private RaycastHit2D _lastControllerColliderHit;
	public Vector3 _velocity;

	public static bool airborne = false;
	private Vector2 playerSize, playerExtents;

	public ParticleSystem dashPS;
	private float dashTimeMax = .5f, dashTime = 0.0f;
	private bool dashing = false;
	private float dashSpeed = 10f;
	private bool canDash = true;

	void Awake()
	{
		GetComponent <SpriteRenderer>().color = Palette.Invisible;

		NotificationMaster.restartObservers.Add (this);

		_animate = GetComponent<Animate>();
		_controller = GetComponent<CharacterController2D>();

		PlayerTransform = transform;
		playerSize = GetComponent <SpriteRenderer> ().bounds.size;
		playerExtents = GetComponent <SpriteRenderer> ().bounds.extents;
	}

	public void Start() {
		SpawnPlayer ();
	}

	private float x, y;
	private bool jump, jumpCancel, dash;
	void Update()
	{
		if (inputDisabled) {
			_velocity = new Vector2 ();
			return;
		}

		x = Input.GetAxis("Horizontal");
		y = Input.GetAxis ("Vertical");

		jump = Input.GetKeyDown (KeyCode.JoystickButton18) || Input.GetKeyDown (KeyCode.UpArrow);
		jumpCancel = Input.GetKeyUp (KeyCode.JoystickButton18) || Input.GetKeyUp (KeyCode.UpArrow);

		dash = Input.GetAxisRaw ("Fire") != 0.0f;

		if (_controller.isGrounded)
			MoveOnGround ();
		else
			MoveInAir ();

		_controller.move( _velocity * Time.deltaTime );
		_velocity = _controller.velocity;
	}

	private void MoveInAir() {
		airborne = true;

		bool colliding = _controller.collisionState.right || _controller.collisionState.left;
		if (!colliding && dashing && dashTime <= dashTimeMax) {
			dashTime += Time.deltaTime;
			_velocity = (dashSpeed) * new Vector2 (x, y).normalized;
		} else {
			dashing = false;
			dashTime = 0.0f;
			if (dashPS.isPlaying)
				dashPS.Stop ();

			_velocity.y += gravity * Time.deltaTime;

			if (Mathf.Abs (x) > .3) {
				normalizedHorizontalSpeed = x;
				if (spriteFlipped * x < 0)
					FlipPlayer ();
			} else
				normalizedHorizontalSpeed = 0;

			var smoothedMovementFactor = groundDamping;
			_velocity.x = Mathf.Lerp (_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
		}
			
		if (!dashing && dash && canDash) {
			canDash = false;
			dashing = true;
			dashPS.Play ();
			AudioManager.PlayPlayerJump ();
		} else if (jumpCancel && (_velocity.y > 0))
			_velocity.y *= .5f;
	}

	private void MoveOnGround() {
		airborne = false;
		canDash = true;
		dashing = false;
		dashTime = 0.0f;
		if (dashPS.isPlaying) {
			dashPS.Stop ();
		}

		_velocity.y = 0;

		if (Mathf.Abs (x) > .3) {
			normalizedHorizontalSpeed = x;
			if (spriteFlipped * x < 0)
				FlipPlayer ();
		} else
			normalizedHorizontalSpeed = 0;

		if (jump) {
			_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
			AudioManager.PlayPlayerJump ();
		}

		var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp (_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
		_velocity.y += gravity * Time.deltaTime;

		if(Input.GetKey(KeyCode.DownArrow))
		{
			_velocity.y *= 3f;
			_controller.ignoreOneWayPlatformsThisFrame = true;
		}
	}
		
	public void FlipPlayer() {
		AudioManager.PlayPlayerTurn ();
		transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		spriteFlipped *= -1;
	}

	private void SpawnPlayer() {
		_velocity = new Vector2 ();

		inputDisabled = true;
		GetComponent <SpriteRenderer>().color = Palette.Invisible;

		Vector3 newPosition = Vector3.zero;
		RaycastHit2D hit;
		// Uhh this is hacky but whatever
		for(int i = 0; i < 5; i++) {
			hit = Physics2D.Linecast (transform.position + new Vector3(0, playerExtents.y, 0), newPosition - new Vector3(0, playerExtents.y * 2, 0), 1 << LayerMask.NameToLayer("Wall"));
			if (hit.collider) {
				newPosition.y = hit.transform.position.y + hit.transform.GetComponent<SpriteRenderer> ().bounds.extents.y + playerExtents.y;
				transform.position = newPosition;
				break;
			}
		}

		_animate.AnimateToColor (Palette.Invisible, Palette.PlayerColor, .5f, Animate.RepeatMode.Once);

		Invoke ("EnableInput", .5f);
	}

	private void EnableInput() {
		inputDisabled = false;
	}

	public void Restart() {
		_velocity = Vector3.zero;
		SpawnPlayer ();
		gameObject.SetActive (true);
	}

	private static Transform _playerTransform;
	public static Transform PlayerTransform
	{
		set { if(_playerTransform == null) _playerTransform = value; }
	}
	public static Vector3 PlayerPosition {
		get { return _playerTransform.position; }
	}
}
