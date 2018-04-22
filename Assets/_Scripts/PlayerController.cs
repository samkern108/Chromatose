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
        private LightAnimate _lightAnimate;
        private Color lightColor;
        public static PlayerController self;
        private SpriteRenderer spriteRenderer;

        [EventID]
        public string eventID;

        public static bool inputDisabled = false, invulnerable = false;

        // movement config
        private float gravity = -32f;

        private Vector3 gravityDirection = Vector3.down;
        private float runSpeed = 6f, dashSpeed = 20f;
        private float jumpHeight = 14f;
        private float groundDamping = 20f; // how fast do we change direction? higher means faster
        private float dashTimeMax = .50f, dashTime = 0.0f;
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

        private Vector2 spawnPosition = new Vector2(0, -4);

        void Awake()
        {
            self = this;

            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Palette.Invisible;

            NotificationMaster.restartObservers.Add(this);

            _animate = GetComponent<Animate>();
            _controller = GetComponent<CharacterController2D>();

            _regularScale = transform.localScale;

            PlayerTransform = transform;
            playerSize = spriteRenderer.bounds.size;
            playerExtents = spriteRenderer.bounds.extents;

            _lightAnimate = GetComponentInChildren<LightAnimate>();
            lightColor = Color.white;

            spawnPosition = transform.position;

            //Koreographer.Instance.RegisterForEventsWithTime(eventID, BeatEvent);
        }

        void Start()
        {
            playerColor = Level.levelPalette.playerColor;
            SpawnPlayer();
        }

        public void Dash()
        {
            canDash = false;
            dashing = true;
            invulnerable = true;
            _lightAnimate.AnimateToIntensity(3f, .1f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(2f, .1f, RepeatMode.Once);
            dashTime = 0.0f;
            dashPS.Play();
            AudioManager.PlayPlayerDash();
            Camera.main.GetComponent<CameraControl>().Shake(.075f, 20, 20);
        }

        private void StopDashInvuln()
        {
            _animate.StopAnimating();
            dashPS.Stop();
            invulnerable = false;
            canDash = true;
            _lightAnimate.AnimateToIntensity(2f, .2f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(1.2f, .2f, RepeatMode.Once);
        }

        private float x, y;
        private bool jump, jumpCancel, dash, dashDown;
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
            dashDown = Input.GetKey(KeyCode.Space);

            bool colliding = _controller.collisionState.right || _controller.collisionState.left ||
                            _controller.collisionState.above || _controller.collisionState.below;

            if (!dashing && dash && canDash)
            {
                Dash();
            }
            else if (dashDown &&  dashing && dashTime <= dashTimeMax)
            {
                dashTime += Time.deltaTime;
                _velocity = (dashSpeed) * new Vector2(x, y).normalized;
            }
            else
            {
                if (dashing)
                {
                    _velocity.y /= 3.0f;
                    dashing = false;
                    StopDashInvuln();
                    //Invoke("StopDashInvuln", .05f);
                }
                else if (_controller.isGrounded) MoveOnGround();
                else MoveInAir();
            }

           // Animate();

            /*RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 10.0f, 1 << LayerMask.NameToLayer("Wall"));
            if (hit)
            {
                _controller.SetUpDirection(hit.normal);
            }*/

            /*float minDistance = Int16.MaxValue;
            float downAngle = 0;

            for (int angle = 0; angle < 360; angle += 180) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down.Rotate2D(angle), 30.0f, 1 << LayerMask.NameToLayer("Wall"));
                if(hit.distance < minDistance) {
                    minDistance = hit.distance;
                    downAngle = angle;
                }
            }
            if(downAngle != gravityAngle) {
                Debug.Log(downAngle + "   " + gravityAngle);
                gravityAngle = downAngle;
                _controller.unrotatedVelocity = _controller.velocity;
            }*/
            _controller.move(_velocity * Time.deltaTime, gravityAngle);
            _velocity = _controller.unrotatedVelocity;
        }

        float gravityAngle = 0;

        private void Animate()
        {
            Vector3 newScale = transform.localScale;
            newScale.x = x;
            newScale.y = y;
            transform.localScale = newScale;
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
                _velocity.y = jumpHeight;//Mathf.Sqrt(jumpHeight * -gravity);
                AudioManager.PlayPlayerJump();
                /*Vector3 animateEndSize = transform.localScale;
                animateEndSize.y *= 1.4f;
                animateEndSize.x *= .8f;
                _animate.AnimateToSize(transform.localScale, animateEndSize, .3f, RepeatMode.OnceAndBack);*/
            }

            var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
            _velocity.y += gravity * Time.deltaTime;
        }

        public void FlipPlayer()
        {
            AudioManager.PlayPlayerTurn();
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            spriteFlipped *= -1;
        }

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
            get { return _playerTransform; }
        }
        public static Vector3 PlayerPosition
        {
            get { return _playerTransform.position; }
        }

        public void Hit()
        {
            health--;
            _animate.AnimateToColor(Level.levelPalette.playerColor, Color.red, .1f, RepeatMode.PingPong);
            lightColor.r /= 3f;
            lightColor.g /= 3f;
            lightColor.b /= 3f;
            _lightAnimate.AnimateToColor(lightColor, Level.secondsPerBeat * 4.0f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(3, Level.secondsPerBeat * 2.0f, RepeatMode.OnceAndBack);
            Invoke("StopHit", Level.secondsPerMeasure);
            invulnerable = true;
            if(health <= 0) {
                Die();
            }
            else {
                AudioManager.PlayPlayerHit();
                Camera.main.GetComponent<CameraControl>().Shake(.15f, 30, 20);
            }
        }

        private void StopHit() {
            _animate.StopAnimating();
            spriteRenderer.color = Level.levelPalette.playerColor;
            invulnerable = false;
        }

private int health = 4;
private bool dead = false;
        private void Die() {
            if(dead) return;

            AudioManager.PlayPlayerDeath();
            Camera.main.GetComponent<CameraControl>().Shake(.2f, 60, 20);
            NotificationMaster.SendPlayerDeathNotification();
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Level.self.PlayerRespawn();
        }
    }
}