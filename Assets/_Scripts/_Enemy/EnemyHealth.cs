using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public interface INotifyOnDeathObserver
    {
        void NotifyOnDeath(EnemyHealth health);
    }

    public interface INotifyOnHitObserver
    {
        void NotifyOnHit(EnemyHealth health, bool dead);
    }

    public class EnemyHealth : MonoBehaviour
    {
        public bool invulnerable = false;
        public bool progressImmediate = false;
        private LightAnimate _lightAnimate;
        private Light _light;
        private Color startingColor;
        private Vector3 normalScale;
        public int healthMax = 3;
        private int currentHealth;
        public GameObject deathExplosion;

        private Animate animate;

        private Color baseColor, hitColor;

        public List<INotifyOnDeathObserver> notifyDelegates = new List<INotifyOnDeathObserver>();
        public List<INotifyOnHitObserver> notifyOnHitDelegates = new List<INotifyOnHitObserver>();

        private bool dead = false;
        private bool inactive = true;

        public EnemyHealth hitListener;

        public void Awake()
        {
            animate = GetComponent<Animate>();
            currentHealth = healthMax;

            baseColor = Color.white;

            normalScale = new Vector3(4, 4, 0);

            _lightAnimate = GetComponentInParent<LightAnimate>();

            notifyOnHitDelegates.Add(BackgroundColor.self);
        }

        public void LoadingStage()
        {
            if (inactive)
            {
                animate.AnimateToSize(Vector3.zero, normalScale, Level.secondsPerMeasure, RepeatMode.Once);
                if (_lightAnimate)
                {
                    _light = _lightAnimate.GetComponent<Light>();
                    startingColor = _light.color;
                    float lightRange = _light.range;
                    float lightIntensity = _light.intensity;
                    _light.range = 0.0f;
                    _light.color = Color.black;
                    _light.intensity *= 6f;
                    _lightAnimate.AnimateToRange(lightRange, Level.secondsPerMeasure, RepeatMode.Once);
                    _lightAnimate.AnimateToColor(Color.white, Level.secondsPerBeat * 8f, RepeatMode.Once);
                    _lightAnimate.AnimateToIntensity(lightIntensity, Level.secondsPerBeat * 2f, RepeatMode.Once);
                }
                inactive = false;
            }
            else
            {
                Invoke("StageBegin", Level.secondsPerMeasure * 2.0f);
            }
        }

        private void SetLightIntensity(float ratio)
        {
            if (_lightAnimate)
            {
                Color color = new Color();
                color.r = startingColor.r * ratio;
                color.g = startingColor.g * ratio;
                color.b = startingColor.b * ratio;
                _lightAnimate.AnimateToColor(color, Level.secondsPerBeat, RepeatMode.Once);
            }
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (dead) return;

            if (col.gameObject.tag == "Player")
            {
                if (PlayerController.dashing && !invulnerable)
                {
                    currentHealth--;
                    SetLightIntensity((float)currentHealth / (float)healthMax);
                    Vector3 currentSize = transform.localScale;
                    animate.AnimateToSize(currentSize, (currentSize - currentSize * .2f), Level.secondsPerBeat, RepeatMode.Once);
                    AudioManager.PlayEnemyHit((float)currentHealth / (float)healthMax);
                    Camera.main.GetComponent<CameraControl>().Shake(.15f, 30, 20);
                    if (currentHealth <= 0 && (hitListener == null || hitListener.dead == false))
                    {
                        Die();
                    }
                    if (notifyOnHitDelegates.Count > 0)
                        foreach (INotifyOnHitObserver obs in notifyOnHitDelegates) {
                            if(obs == null) continue;
                            obs.NotifyOnHit(this, dead);
                        }
                }
                else if (!PlayerController.invulnerable)
                {
                    PlayerController.self.Hit();
                }
            }
        }

        public void Die()
        {
            if (!dead)
            {
                animate.AnimateToColor(StageConstants.self.enemyHit1, StageConstants.self.enemyHit2, Level.secondsPerBeat * .05f, RepeatMode.PingPong, AnimPriority.Critical);
                Vector3 currentSize = transform.localScale;
                animate.AnimateToSize(currentSize, (currentSize - currentSize * .2f), Level.secondsPerBeat * 2.0f, RepeatMode.Once);
                Invoke("Destroy", Level.secondsPerBeat * 2.0f);
                dead = true;
                if (_lightAnimate) _lightAnimate.Die();
            }
        }

        public void StageBegin()
        {
            if (currentHealth != healthMax)
            {
                currentHealth = healthMax;
                SetLightIntensity(1);
                animate.AnimateToSize(transform.localScale, normalScale, Level.secondsPerBeat * 2.0f, RepeatMode.Once);
            }
        }

        private void Destroy()
        {
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
            if (notifyDelegates.Count > 0)
                foreach (INotifyOnDeathObserver obs in notifyDelegates)
                    obs.NotifyOnDeath(this);

            if (_lightAnimate) GameObject.Destroy(_lightAnimate);
            GameObject.Destroy(this.gameObject);
        }
    }
}