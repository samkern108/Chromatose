using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chromatose
{
    public class UIManager : MonoBehaviour
    {
        public bool tutorialEnabled = true;
        private int exitsCompleted = 0;

        public GameObject introPanel, controlsPanel;

        public void Start()
        {
            if (tutorialEnabled)
            {
                introPanel.SetActive(true);
            }
            else
            {
                introPanel.SetActive(false);
                Level.self.StartGameDelayed();
            }
            controlsPanel.SetActive(false);
        }

        public void Update()
        {
            if (introPanel.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton16) || Input.GetKeyDown(KeyCode.Space))
                {
                    introPanel.SetActive(false);
                    controlsPanel.SetActive(true);
                }
            }
            else if (controlsPanel.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton16) || Input.GetKeyDown(KeyCode.Space))
                {
                    controlsPanel.SetActive(false);
                    Level.self.StartGameDelayed();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button9))
            {

            }
            else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Joystick1Button10))
            {

            }
        }
    }
}