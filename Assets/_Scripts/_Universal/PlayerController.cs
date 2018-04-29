using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Prime31;
using SonicBloom.Koreo;

namespace Chromatose
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController self;

        public static bool inputDisabled = false, invulnerable = false;

        // movement config
        private float gravity = -32f;
        private float runSpeed = 6f, dashSpeed = 20f;
        private float jumpHeight = 14f;
        private float groundDamping = 20f; // how fast do we change direction? higher means faster
        private float dashTimeMax = .50f, dashTime = 0.0f, dashCooldownMax = .1f, dashCooldown = 0.0f;
        public static bool dashing = false, canDash = true, airborne = false;

        private int maxHealth = 4, health = 4;
        private bool dead = false;

        private float normalizedHorizontalSpeed = 0;

        // A non-flipped PlayerTemp sprite faces right (1)
        public static int spriteFlipped = 1;

        public Vector3 _velocity;

        private CharacterController2D _controller;

        private Vector2 spawnPosition = new Vector2(0, -4);

        public List<INotifyOnHitObserver> hitObservers = new List<INotifyOnHitObserver>();

        void Awake()
        {
            self = this;

            _controller = GetComponent<CharacterController2D>();
            PlayerTransform = transform;

            spawnPosition = transform.position;
            inputDisabled = true;
        }

        void Start()
        {
            hitObservers.Add(BackgroundColor.self);
        }

        public void Dash()
        {
            canDash = false;
            dashing = true;
            invulnerable = true;
            dashTime = 0.0f;
            PlayerEffects.Dash();
        }

        private void StopDashInvuln()
        {
            invulnerable = false;
            canDash = true;
            PlayerEffects.StopDash();
        }

        private float lastX, lastY, x, y, xRaw, yRaw;
        private bool jump, jumpCancel, dash, dashDown;
        private Vector3 lastDashDirection;
        float lastXRaw, lastYRaw;

        void Update()
        {
            if (inputDisabled)
            {
                _velocity = new Vector2();
                return;
            }

            lastX = x;
            lastY = y;
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");

            lastXRaw = xRaw;
            lastYRaw = yRaw;
            xRaw = Input.GetAxisRaw("Horizontal");
            yRaw = Input.GetAxisRaw("Vertical");

            jump = Input.GetKeyDown(KeyCode.JoystickButton18) || Input.GetKeyDown(KeyCode.UpArrow);
            jumpCancel = Input.GetKeyUp(KeyCode.JoystickButton18) || Input.GetKeyUp(KeyCode.UpArrow);
            dash = Input.GetKeyDown(KeyCode.JoystickButton16) || Input.GetKeyDown(KeyCode.Space);
            dashDown = Input.GetKey(KeyCode.JoystickButton16) || Input.GetKey(KeyCode.Space);

            bool colliding = _controller.collisionState.right || _controller.collisionState.left ||
                            _controller.collisionState.above || _controller.collisionState.below;

            dashCooldown -= Time.deltaTime;
            if (!dashing && dash && canDash && dashCooldown <= 0.0f)
            {
                Dash();
            }
            else if ((dashDown || dashTime <= dashTimeMax / 2.0f) && dashing && (dashTime <= dashTimeMax))
            {
                dashTime += Time.deltaTime;
                Vector3 dashDirection = new Vector2((x + lastX) / 2.0f, (y + lastY) / 2.0f).normalized;
                if(xRaw == 0 && yRaw == 0)
                    dashDirection = Vector2.zero;
                _velocity = dashSpeed * dashDirection;

                float percentage = 1 - (dashTime)/ (dashTimeMax);
                float percentageOffset = percentage / 2.0f + .5f;
                PlayerEffects.UpdateDash(percentage);
            }
            else
            {
                if (dashing)
                {
                    _velocity.y /= 3.0f;
                    dashing = false;
                    dashCooldown = dashCooldownMax;
                    StopDashInvuln();
                }
                else if (_controller.isGrounded) MoveOnGround();
                else MoveInAir();
            }
            _controller.move(_velocity * Time.deltaTime, gravityAngle);
            _velocity = _controller.unrotatedVelocity;
        }

        float gravityAngle = 0;

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
            if (airborne)
            {
                airborne = false;
                PlayerEffects.LandOnGround();
            }

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
                _velocity.y = jumpHeight;
                PlayerEffects.Jump();
            }

            var smoothedMovementFactor = groundDamping;//_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
            _velocity.y += gravity * Time.deltaTime;
        }

        public void FlipPlayer()
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            spriteFlipped *= -1;
        }

        private void SpawnPlayer()
        {
            invulnerable = true;
            inputDisabled = true;

            PlayerEffects.Spawn();

            _velocity = new Vector2();
            transform.position = spawnPosition;

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
            if (health != maxHealth)
            {
                health = maxHealth;
                PlayerEffects.RestoreHealth();
            }

            _velocity = Vector3.zero;
            SpawnPlayer();
            gameObject.SetActive(true);
        }

        public void Hit()
        {
            health--;

            if (health <= 0)
            {
                foreach (INotifyOnHitObserver obs in hitObservers)
                    obs.NotifyOnHit(null, true);
                Die();
            }
            else
            {
                foreach (INotifyOnHitObserver obs in hitObservers)
                    obs.NotifyOnHit(null, false);
                invulnerable = true;
                PlayerEffects.Hit();
                Invoke("StopHit", Level.secondsPerMeasure);
            }
        }

        private void StopHit()
        {
            PlayerEffects.StopHit();
            invulnerable = false;
        }

        private void Die()
        {
            if (dead) return;

            PlayerEffects.Die();
            NotificationMaster.SendPlayerDeathNotification();
            gameObject.SetActive(false);
            Level.self.PlayerRespawn();
        }

        public void LoadingStage()
        {
            if (health != maxHealth)
            {
                health = maxHealth;
                PlayerEffects.RestoreHealth();
            }
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
    }
}