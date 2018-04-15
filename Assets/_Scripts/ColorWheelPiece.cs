using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class ColorWheelPiece : MonoBehaviour
    {

        private Animate animate;
        public float angle;
        public Color color;
        public static int pieceCounter = 0;
        public int pieceIndex = 0;

        void Start()
        {
            color = GetComponent<SpriteRenderer>().color;
            animate = GetComponent<Animate>();

            /*Vector3 startPosition = .6f * transform.up;
            Vector3 finalPosition = 1.0f * transform.up;

            Vector3 startScale = .6f * transform.localScale;
            Vector3 finalScale = .4f * transform.localScale;*/

            Vector3 startPosition = 1.2f * transform.up;
            Vector3 finalPosition = 2.0f * transform.up;

            Vector3 startScale = 1.2f * transform.localScale;
            Vector3 finalScale = .8f * transform.localScale;

            animate.AnimateToPosition(startPosition, finalPosition, 8.0f, RepeatMode.PingPong);
            animate.AnimateToSize(startScale, finalScale, 8.0f, RepeatMode.PingPong);
            pieceIndex = pieceCounter;
            pieceCounter++;
        }

        public void OnMouseEnter()
        {
            animate.AnimateToColor(color, Color.white, .2f, RepeatMode.Once);
        }

        public void OnMouseExit()
        {
            animate.AnimateToColor(Color.white, color, .2f, RepeatMode.Once);
        }

        public void OnMouseDown()
        {
            ColorWheel.self.PieceClicked(this);
        }
    }
}