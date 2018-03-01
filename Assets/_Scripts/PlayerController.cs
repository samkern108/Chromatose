using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;
using SonicBloom.Koreo;

public class PlayerController : MonoBehaviour, IRestartObserver {

	[EventID]
	public string eventID;

	private bool inputDisabled = false;

	// movement config
	private float gravity = -20f;//-36f;
	private float gravityRotation = 0f;
	private float runSpeed = 7f;
	private float groundDamping = 20f; // how fast do we change direction? higher means faster
	private float inAirDamping = 5f;
	private float jumpHeight = 7f;

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
	private float dashSpeed = 16f;
	private bool canDash = true;

	void Awake()
	{
		//InvokeRepeating ("RotateGravity", 4.0f, 4.0f);
		//RotateGravity();
		GetComponent <SpriteRenderer>().color = Palette.Invisible;

		NotificationMaster.restartObservers.Add (this);

		_animate = GetComponent<Animate>();
		_controller = GetComponent<CharacterController2D>();

		PlayerTransform = transform;
		playerSize = GetComponent <SpriteRenderer> ().bounds.size;
		playerExtents = GetComponent <SpriteRenderer> ().bounds.extents;

		Koreographer.Instance.RegisterForEventsWithTime(eventID, BeatEvent);
	}

	void Start() {
		dashTime = Level.secondsPerMeasure;
		SpawnPlayer ();
	}

	private bool waitingForBoostCheck = false;
	private int lastSampleTime;
	private int boostSampleTimeGracePeriod = 5000;
	private int lastBoostSampleTime = -int.MaxValue;
	public void BeatEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice) {
		lastSampleTime = sampleTime;
		float val = Math.Abs (lastBoostSampleTime - lastSampleTime);
		if (val/boostSampleTimeGracePeriod <= 1.0f) {
			canDash = false;
			dashing = true;
			dashTime = 0.0f;
			dashPS.Play ();
			AudioManager.PlayPlayerJump ();
			Camera.main.GetComponent <ScreenShake>().Pulse(val);
		}
		waitingForBoostCheck = false;
	}

	public void CompareDashSampleTime() {
		lastBoostSampleTime = Koreographer.Instance.GetMusicSampleTime ();
		float val = Math.Abs (lastBoostSampleTime - lastSampleTime);
		if (val/boostSampleTimeGracePeriod <= 1.0f) {
			canDash = false;
			dashing = true;
			dashTime = 0.0f;
			dashPS.Play ();
			AudioManager.PlayPlayerJump ();
			Camera.main.GetComponent <ScreenShake>().Pulse(val);
			waitingForBoostCheck = false;
		}
	}
		
	private int beatCounter;
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

		bool colliding = _controller.collisionState.right || _controller.collisionState.left;

		if (!dashing && dash && canDash && !waitingForBoostCheck) {
			lastBoostSampleTime = Level.koreo.GetLatestSampleTime ();
			waitingForBoostCheck = true;
			CompareDashSampleTime ();
		} else if (!colliding && dashing && dashTime <= dashTimeMax && (dash || dashTime < dashTimeMax/2.0f)) {
			dashTime += Time.deltaTime;
			_velocity = (dashSpeed) * new Vector2 (x, y).normalized;
		}
		else {
			dashing = false;
			if (dashPS.isPlaying)
				dashPS.Stop ();

			if (_controller.isGrounded)
				MoveOnGround ();
			else
				MoveInAir ();
		}
			
		RaycastHit2D hit = Physics2D.Raycast (transform.position, -transform.up, 10.0f, 1 << LayerMask.NameToLayer("Wall"));
		if (hit) {
			_controller.SetUpDirection (hit.normal);
		}

		RotateGravity (Time.deltaTime * 5);
		_controller.move( _velocity * Time.deltaTime * 0);

		_velocity = _controller.velocity;
	}

	private void RotateGravity(float rotation) {
		gravityRotation += rotation;//(gravityRotation + 90) % 360;
		transform.localRotation = Quaternion.Euler (new Vector3(0, 0, gravityRotation));
	}

	private void MoveInAir() {
		airborne = true;

		_velocity.y += gravity * Time.deltaTime;

		if (Mathf.Abs (x) > .3) {
			normalizedHorizontalSpeed = x;
			if (spriteFlipped * x < 0)
				FlipPlayer ();
		} else
			normalizedHorizontalSpeed = 0;

		var smoothedMovementFactor = groundDamping;
		_velocity.x = Mathf.Lerp (_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

	 	if (jumpCancel && (_velocity.y > 0))
			_velocity.y *= .5f;
	}

	private void MoveOnGround() {
		airborne = false;
		if(!dashing)
			canDash = true;

		_velocity.y = 0;

		if (Mathf.Abs (x) > .3) {
			normalizedHorizontalSpeed = x;
			if (spriteFlipped * x < 0)
				FlipPlayer ();
		} else
			normalizedHorizontalSpeed = 0;

		if (jump) {
			_velocity.y = Mathf.Sqrt (jumpHeight * -gravity);
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
	
		//_controller.warpToGrounded ();

		_animate.AnimateToColor (Palette.Invisible, Color.white, .5f, Animate.RepeatMode.Once);

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
