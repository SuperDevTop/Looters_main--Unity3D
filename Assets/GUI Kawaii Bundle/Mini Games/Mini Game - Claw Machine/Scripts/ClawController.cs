using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClawMachine
{
    public class ClawController : MonoBehaviour
    {
        [Header("UI Buttons")]
        [Space]
        public GameObject pullButton;
        public GameObject clawMoveButtonL, clawMoveButtonR;

        [Header("Claw Settings")]
        [Space]
        public GameObject clawInside;
        public GameObject clawArmL, clawArmR;
        public int xMinValue, xMaxValue;

        private float startPositionY;
        private string moveDirection;
        private bool isPull;
        private bool isClawMoving;

        private void Start()
        {
            startPositionY = transform.localPosition.y;
        }
        private void Update()
        {
            if (isClawMoving && moveDirection == "Left")
            {
                if (transform.localPosition.x > xMinValue)
                    transform.localPosition += new Vector3(-10, 0, 0);
            }
            else if (isClawMoving && moveDirection == "Right")
            {
                if (transform.localPosition.x < xMaxValue)
                    transform.localPosition += new Vector3(10, 0, 0);
            }
        }

        public void ControlButtons(string direction)
        {
            switch (direction)
            {
                case "Left":
                    {
                        moveDirection = "Left";
                        isClawMoving = true;
                        break;
                    }
                case "Right":
                    {
                        moveDirection = "Right";
                        isClawMoving = true;
                        break;
                    }
            }
        }

        public void ControlButtonPointUp()
        {
            isClawMoving = false;
        }

        public void PullingReward()
        {
            isPull = true;
            StartCoroutine(ClawVerticalMovement(true));
            StartCoroutine(ClawArmsRoutine(true));
        }

        IEnumerator ClawVerticalMovement(bool isGoingDown)
        {
            if (isGoingDown)
            {
                ClawControlButtonsSettings(false);

                while (isPull)
                {
                    transform.localPosition -= new Vector3(0, 7f, 0);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                while (transform.localPosition.y < startPositionY) //Go Up
                {
                    transform.localPosition += new Vector3(0, 7f, 0);
                    yield return new WaitForSeconds(0.01f);
                }

                GameController.ins.RewardsCollected = true;
                ActivateInSideCollider(true);
                yield return new WaitForSeconds(1f); //To give a time for all rewards inside claw detected.
                GameController.ins.RewardResults();

            }

        }

        IEnumerator ClawArmsRoutine(bool isOpenning)
        {
            if (isOpenning)
            {
                for (int z = 0; z < 45; z++)
                {
                    clawArmL.transform.Rotate(new Vector3(0, 0, clawArmL.transform.localRotation.z - 1), 1f);
                    clawArmR.transform.Rotate(new Vector3(0, 0, clawArmL.transform.localRotation.z + 1), 1f);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            else
            {
                for (int z = 0; z < 45; z++)
                {
                    clawArmL.transform.Rotate(new Vector3(0, 0, clawArmL.transform.localRotation.z + 1), 1f);
                    clawArmR.transform.Rotate(new Vector3(0, 0, clawArmL.transform.localRotation.z - 1), 1f);
                    yield return new WaitForSeconds(0.01f);
                }

                StartCoroutine(ClawVerticalMovement(false));
            }

        }

        public void ClawControlButtonsSettings(bool isActive)
        {
            clawMoveButtonL.GetComponent<Image>().raycastTarget = isActive;
            clawMoveButtonR.GetComponent<Image>().raycastTarget = isActive;
            pullButton.GetComponent<Button>().interactable = isActive;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isPull)
            {
                isPull = false;
                StartCoroutine(ClawArmsRoutine(false));
            }
        }

        public void ClawPositionReset()
        {
            transform.localPosition = new Vector3(0, startPositionY, 0);
            ActivateInSideCollider(false);
            ClawControlButtonsSettings(true);
            ControlButtonPointUp();
        }

        public void ActivateInSideCollider(bool isActive)
        {
            clawInside.GetComponent<PolygonCollider2D>().enabled = isActive;
        }
    }
}