using UnityEngine;

namespace SlotMachine
{
    public class UIProfileController : MonoBehaviour
    {
        public enum RewardEnum                  // Your Custom Reward Category
        {
            None,
            Coin,
            Diamond,
            Energy,
            Life,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };
        [System.Serializable]
        public class MultiDimensional
        {
            public RewardEnum rewardType;
            public GameObject UIElement;
        }
        public MultiDimensional[] UIProfileElements;

        private void Start()
        {
            SetActiveElements();
        }

        private void SetActiveElements()
        {
            for (int i = 0; i < GameController.ins.SlotTypes.Length; i++)
            {
                GameController.RewardEnum collectedRewardType = GameController.ins.SlotTypes[i].rewardCategory;

                switch (collectedRewardType)
                {
                    case GameController.RewardEnum.Coin:
                        {
                            FindThatElement(RewardEnum.Coin);
                        }
                        break;
                    case GameController.RewardEnum.Gem4:
                        {
                            FindThatElement(RewardEnum.Diamond);
                        }
                        break;
                    default:
                        UIProfileElements[i].UIElement.SetActive(false);
                        break;
                }
            }
        }


        private void FindThatElement(RewardEnum type)
        {
            for (int i = 0; i < UIProfileElements.Length; i++)
            {
                if (UIProfileElements[i].rewardType == type)
                    UIProfileElements[i].UIElement.SetActive(true);
            }
        }
    }
}