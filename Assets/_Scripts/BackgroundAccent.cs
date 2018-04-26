using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class BackgroundAccent : MonoBehaviour
    {

        private Animate _animate;
        private SpriteRenderer _spriteRenderer;
        private Color normalColor;

        void Awake()
        {
            _animate = GetComponent<Animate>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            normalColor = _spriteRenderer.color;
            normalColor.a = .2f;
            _spriteRenderer.color = normalColor;
            Invoke("AnimateBG", 4.0f);
        }

        private void AnimateBG()
        {
            Color color1 = normalColor;
            Color color2 = color1;
            color1.a = .1f;
            color2.a = .8f;
            float time = Level.secondsPerMeasure * (1.0f / LevelMusic.audioSource.pitch);
            _animate.AnimateToColor(color1, color2, time, RepeatMode.OnceAndBack);
            Invoke("AnimateBG", (time * 2.0f));
        }

        private Color accentColor;

        public void OverrideColor(Color newAccentColor)
        {
            accentColor = newAccentColor;
            float time = 2.0f * Level.secondsPerBeat * (1.0f / LevelMusic.audioSource.pitch);
            _animate.AnimateToColor(_spriteRenderer.color, Color.white, time, RepeatMode.Once);
            Invoke("WhiteToAccent", time);
        }

        private void WhiteToAccent()
        {
            float time = 2.0f * Level.secondsPerBeat * (1.0f / LevelMusic.audioSource.pitch);
            _animate.AnimateToColor(Color.white, accentColor, time, RepeatMode.Once);
            Invoke("AccentToWhite", time);
        }

        private void AccentToWhite()
        {
            float time = 2.0f * Level.secondsPerBeat * (1.0f / LevelMusic.audioSource.pitch);
            _animate.AnimateToColor(accentColor, Color.white, time, RepeatMode.Once);
            Invoke("EndColorOverride", time);
        }

        private void EndColorOverride()
        {
            float time = 2.0f * Level.secondsPerBeat * (1.0f / LevelMusic.audioSource.pitch);
            _animate.AnimateToColor(Color.white, normalColor, time, RepeatMode.Once);
            Invoke("AnimateBG", time);
        }
    }
}