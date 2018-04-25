using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromatose
{
    public class ColorWheelDotCollider : MonoBehaviour
    {
		private Color color;
		private ColorWheelPiece myParent;

		public void Start() {
			color = GetComponent<SpriteRenderer>().color;
			myParent = GetComponentInParent<ColorWheelPiece>();
		}
        public void OnMouseEnter()
        {
			myParent.OnMouseEnter();
        }

        public void OnMouseExit()
        {
			myParent.OnMouseExit();
        }

        public void OnMouseDown()
        {
			myParent.OnMouseDown();
        }
    }
}