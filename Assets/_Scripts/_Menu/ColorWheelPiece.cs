using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class ColorWheelPiece : MonoBehaviour
    {
        private Animate[] animates;
        public float angle;
        public Color color;
        public static int pieceCounter = 0;
        public int pieceIndex = 0;
        public static int selectedPiece = 0;
        public static float pieceSelectCooldown = 0f;

        public bool selectable = false;

        public GameObject activatedSprite;

        void Start()
        {
            color = GetComponent<SpriteRenderer>().color;
            animates = GetComponentsInChildren<Animate>();

            Vector3 startPosition = 1.6f * transform.up;
            Vector3 finalPosition = 2.2f * transform.up;

            Vector3 startScale = 1.0f * transform.localScale;
            Vector3 finalScale = .7f * transform.localScale;

            animates[0].AnimateToPosition(startPosition, finalPosition, 10.0f, RepeatMode.PingPong);
            animates[0].AnimateToSize(startScale, finalScale, 6.0f, RepeatMode.PingPong);
            pieceIndex = pieceCounter;
            pieceCounter++;

            if (Scenes.scenes[pieceIndex].sceneEnabled)
            {
                activatedSprite.GetComponent<SpriteRenderer>().color = color;
                activatedSprite.SetActive(true);
            }
            else {
                activatedSprite.SetActive(false);
            }
        }

        public void OnMouseEnter()
        {
            if (Scenes.scenes[pieceIndex].sceneEnabled)
            {
                foreach(Animate animate in animates)
                    animate.AnimateToColor(color, Color.white, .2f, RepeatMode.Once);
            }
        }

        public void OnMouseExit()
        {
            if (Scenes.scenes[pieceIndex].sceneEnabled)
            {
                foreach(Animate animate in animates)
                    animate.AnimateToColor(Color.white, color, .2f, RepeatMode.Once);
            }
        }

        public void OnMouseDown()
        {
            if (Scenes.scenes[pieceIndex].sceneEnabled)
            {
                ColorWheel.self.PieceClicked(this, Scenes.scenes[pieceIndex].sceneId);
            }
        }
    }
}