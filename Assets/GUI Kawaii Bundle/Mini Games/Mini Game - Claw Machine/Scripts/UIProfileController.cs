using UnityEngine;

namespace ClawMachine
{
    public class UIProfileController : MonoBehaviour
    {
        public GameObject[] UIProfileElements;

        private void Start()
        {
            SetActiveElements();
        }
        private void SetActiveElements()
        {
            for (int i = 0; i < GameController.ins.Rewards.Length; i++)
            {
                if (GameController.ins.Rewards[i].maxCount > 0) //Activate this currency because it is using
                {
                    UIProfileElements[i].SetActive(true);
                }
                else
                {
                    UIProfileElements[i].SetActive(false);
                }
            }
        }
    }
}
