using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class PlayerController : MonoBehaviour, IRestartObserver
    {
        public static PlayerController self;
        private SpriteRenderer spriteRenderer;
        public Sprite circle, triangle;

        [EventID]
        public string eventID;

        public static bool inputDisabled = false, invulnerable = false;

        // movement config
        private float gravity = -32f, gravityRotation = 0f;
        private float runSpeed = 7f, dashSpeed = 30f, jumpHeight = 7f;
        private float groundDamping = 20f; // how fast do we change direction? higher means faster
        private float dashTimeMax, dashTime = 0.0f;
        public static bool dashing = false, canDash = true, airborne = false;

        private float normalizedHorizontalSpeed = 0;

        // A non-flipped PlayerTemp sprite faces right (1)
        public static int spriteFlipped = 1;

        private CharacterController2D _controller;
        private Animate _animate;
        public Vector3 _velocity;

        private Vector3 _regularScale;

        private Vector2 playerSize, playerExtents;

        public GameObject deathExplosion;

        public ParticleSystem dashPS;

        private Color playerColor;

        void Awake()
        {
            self = this;

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Palette.Invisible;
            playerColor = Level.levelPalette.playerColor;

            NotificationMaster.restartObservers.Add(this);

            _animate = GetComponent<Animate>();
            _controller = GetComponent<CharacterController2D>();

            _regularScale = transform.localScale;

            PlayerTransform = transform;
            playerSize = spriteRenderer.bounds.size;
            playerExtents = spriteRenderer.bounds.extents;

            //Koreographer.Instance.RegisterForEventsWithTime(eventID, BeatEvent);
        }

        void Start()
        {
            dashTimeMax = Level.secondsPerBeat / 4.0f;
            SpawnPlayer();
        }

        public void Dash()
        {
            canDash = false;
            dashing = true;
            spriteRenderer.sprite = triangle;
            dashTime = 0.0f;
            dashPS.Play();
            AudioManager.PlayPlayerJump();
        }

        private float x, y;
        private bool jump, jumpCancel, dash;
        void Update()
        {
            if (inputDisabled)
            {
                _velocity = new Vector2();
                return;
            }

            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");

            jump = Input.GetKeyDown(KeyCode.JoystickButton18) || Input.GetKeyDown(KeyCode.UpArrow);
            jumpCancel = Input.GetKeyUp(KeyCode.JoystickButton18) || Input.GetKeyUp(KeyCode.UpArrow);
            dash = Input.GetKeyDown(KeyCode.Space);

            bool colliding = _controller.collisionState.right || _controller.collisionState.left ||
                            _controller.collisionState.above || _controller.collisionState.below;

            if (!dashing && dash && canDash)
            {
                Dash();
            }
            else if (!colliding && dashing && dashTime <= dashTimeMax)
            {
                dashTime += Time.deltaTime;
                _velocity = (dashSpeed) * new Vector2(x, y).normalized;
            }
            else
            {
                if (dashing)
                {
                    _velocity.y /= 2.0f;
                    dashing = false;
                    spriteRenderer.sprite = circle;
                    if (dashPS.isPlaying)
                        Invoke("StopDashPS", dashTimeMax);
                }

                if (_controller.isGrounded) MoveOnGround();
                else MoveInAir();
            }

           // Animate();

            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10.0f, 1 << LayerMask.NameToLayer("Wall"));
            if (hit)
            {
                _controller.SetUpDirection(hit.normal);
            }

            _controller.move(_velocity * Time.deltaTime);

            _velocity = _controller.velocity;
        }

        private void Animate()
        {
            Vector3 newScale = transform.localScale;
            newScale.x = x;
            newScale.y = y;
            transform.localScale = newScale;
        }

        private void StopDashPS()
        {
            dashPS.Stop();
        }

        private void MoveInAir()
        {
            airborne = true;

            _velocity.y += gravity * Time.deltaTime;

            if (Mathf.Abs(x) > .3)
            {
                normalizedHorizontalSpeed = x;
                if (spriteFlipped * x < 0)
                    FlipPlayer();
            }
            else
                normalizedHorizontalSpeed = 0;

            var smoothedMovementFactor = groundDamping;
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

            if (jumpCancel && (_velocity.y > 0))
                _velocity.y *= .5f;
        }

        private void MoveOnGround()
        {
            airborne = false;
            if (!dashing)
                canDash = true;

            _velocity.y = 0;

            if (Mathf.Abs(x) > .3)
            {
                normalizedHorizontalSpeed = x;
                if (spriteFlipped * x < 0)
                    FlipPlayer();
            }
            else
                normalizedHorizontalSpeed = 0;

            if (jump)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -gravity);
                AudioManager.PlayPlayerJump();
                Vector3 animateEndSize = transform.localScale;
                animateEndSize.y *= 1.4f;
                animateEndSize.x *= .8f;
                _animate.AnimateToSize(transform.localScale, animateEndSize, .3f, RepeatMode.OnceAndBack);
            }

            var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
            _velocity.y += gravity * Time.deltaTime;

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _velocity.y *= 3f;
                _controller.ignoreOneWayPlatformsThisFrame = true;
            }
        }

        public void FlipPlayer()
        {
            AudioManager.PlayPlayerTurn();
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            spriteFlipped *= -1;
        }

        private Vector2 spawnPosition = new Vector2(0, -4);
        private void SpawnPlayer()
        {
            invulnerable = true;
            inputDisabled = true;

            _velocity = new Vector2();
            spriteRenderer.color = Palette.Invisible;

            transform.position = spawnPosition;

            _animate.AnimateToColor(Palette.Invisible, playerColor, .5f, RepeatMode.Once);

            Invoke("EnableInput", 1f);
        }

        private void EnableInput()
        {
            invulnerable = false;
            inputDisabled = false;
        }

        public static void Respawn()
        {
            PlayerController.self.Restart();
        }
        public void Restart()
        {
            _velocity = Vector3.zero;
            SpawnPlayer();
            gameObject.SetActive(true);
        }

        private static Transform _playerTransform;
        public static Transform PlayerTransform
        {
            set { if (_playerTransform == null) _playerTransform = value; }
        }
        public static Vector3 PlayerPosition
        {
            get { return _playerTransform.position; }
        }

        public void Die()
        {
            NotificationMaster.SendPlayerDeathNotification();
            AudioManager.PlayPlayerDeath();
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Level.self.PlayerRespawn();
        }
    }
}