using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class PlayerEffects : MonoBehaviour
    {
        private static Animate _animate;
        private static LightAnimate _lightAnimate;
        private static Light _light;
        private static Vector3 _regularScale;

        private static GameObject deathExplosion;

        private static ParticleSystem dashPS;

        private static Color lightColor, playerColor;
        private static SpriteRenderer spriteRenderer;
        private static Transform spriteTransform, pfxTransform;

        void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteTransform = spriteRenderer.transform;
            spriteRenderer.color = Palette.Invisible;
            _animate = GetComponentInChildren<Animate>();
            _regularScale = spriteTransform.localScale;
            _lightAnimate = GetComponentInChildren<LightAnimate>();
            _light = GetComponentInChildren<Light>();
            lightColor = Color.white;

            _light.color = Color.black;
            _light.range = 0f;

            dashPS = transform.Find("DashPS").GetComponent<ParticleSystem>();
            deathExplosion = transform.Find("Death Explosion").gameObject;
            deathExplosion.transform.SetParent(null);
            deathExplosion.transform.localScale = Vector3.one;

            pfxTransform = this.transform;
        }

        void Start()
        {
            playerColor = Level.levelPalette.playerColor;
        }

        public static void Dash()
        {
            _lightAnimate.AnimateToIntensity(3f, .1f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(2f, .1f, RepeatMode.Once);
            dashPS.Play();
            AudioManager.PlayPlayerDash();
            Camera.main.GetComponent<CameraControl>().Shake(.075f, 20, 20);
        }

        public static void StopDash()
        {
            dashPS.Stop();
            _lightAnimate.AnimateToIntensity(2f, .2f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(1.2f, .2f, RepeatMode.Once);
        }

        public static void Hit()
        {
            _animate.AnimateToColor(StageConstants.self.enemyHit1, StageConstants.self.enemyHit2, .1f, RepeatMode.PingPong);
            lightColor.r /= 3f;
            lightColor.g /= 3f;
            lightColor.b /= 3f;
            _lightAnimate.AnimateToColor(lightColor, Level.secondsPerBeat * 2.0f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(2.6f, Level.secondsPerBeat, RepeatMode.OnceAndBack);
            AudioManager.PlayPlayerHit();
            Camera.main.GetComponent<CameraControl>().Shake(.15f, 30, 20);
        }

        public static void StopHit()
        {
            _animate.StopAnimating(AnimType.Color);
            spriteRenderer.color = Level.levelPalette.playerColor;
        }

        public static void Spawn()
        {
            spriteRenderer.color = Palette.Invisible;
            _animate.AnimateToColor(Palette.Invisible, playerColor, Level.secondsPerBeat * 2f, RepeatMode.Once);
            _light.color = Color.black;
            _light.range = 0f;
            _lightAnimate.AnimateToColor(Color.white, Level.secondsPerBeat * 2f, RepeatMode.Once);
            _lightAnimate.AnimateToRange(1.2f, Level.secondsPerBeat * 2f, RepeatMode.Once);
        }

        public static void Die()
        {
            AudioManager.PlayPlayerDeath();
            Camera.main.GetComponent<CameraControl>().Shake(.2f, 60, 20);
            deathExplosion.transform.position = pfxTransform.position;
            deathExplosion.SetActive(true);
        }

        public static void Jump()
        {
            AudioManager.PlayPlayerJump();
            Vector3 animateEndSize = _regularScale;
            animateEndSize.y *= 1.1f;
            animateEndSize.x *= .95f;
            _animate.AnimateToSize(spriteTransform.localScale, animateEndSize, .2f, RepeatMode.Once);
        }

        public static void LandOnGround()
        {
            Vector3 animateEndSize = _regularScale;
            animateEndSize.y *= .85f;
            animateEndSize.x *= 1.2f;
            _animate.AnimateToSize(_regularScale, animateEndSize, .05f, RepeatMode.OnceAndBack);
        }

        public static void RestoreHealth()
        {
            _lightAnimate.AnimateToColor(Color.white, Level.secondsPerMeasure, RepeatMode.Once);
            _lightAnimate.AnimateToRange(1.2f, Level.secondsPerMeasure, RepeatMode.Once);
            lightColor = Color.white;
        }
    }
}