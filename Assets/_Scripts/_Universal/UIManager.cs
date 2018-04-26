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

        public GameObject introPanelPC, controlsPanelPC, introPanelXBOX, controlsPanelXBOX;
        public bool onXbox = false;

        public void Start()
        {
            introPanelPC.SetActive(false);
            introPanelXBOX.SetActive(false);
            controlsPanelXBOX.SetActive(false);
            controlsPanelPC.SetActive(false);

            if (tutorialEnabled)
            {
                if (onXbox) introPanelXBOX.SetActive(true);
                else introPanelPC.SetActive(true);
            }
            else
            {
                Level.self.StartGameDelayed();
                GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
                foreach (GameObject wall in walls)
                {
                    wall.GetComponent<Wall>().AnimateWallOutward();
                }
            }
        }

        public void Update()
        {
            if (introPanelPC.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
            {
                introPanelPC.SetActive(false);
                controlsPanelPC.SetActive(true);
            }
            else if (introPanelXBOX.activeInHierarchy && Input.GetKeyDown(KeyCode.JoystickButton16))
            {
                introPanelXBOX.SetActive(false);
                controlsPanelXBOX.SetActive(true);
            }
            else if (controlsPanelPC.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
            {
                controlsPanelPC.SetActive(false);

                GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
                foreach (GameObject wall in walls)
                {
                    wall.GetComponent<Wall>().AnimateWallOutward();
                }
                Level.self.StartGameDelayed();
            }
            else if (controlsPanelXBOX.activeInHierarchy && Input.GetKeyDown(KeyCode.JoystickButton16))
            {
                controlsPanelXBOX.SetActive(false);

                GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
                foreach (GameObject wall in walls)
                {
                    wall.GetComponent<Wall>().AnimateWallOutward();
                }
                Level.self.StartGameDelayed();
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