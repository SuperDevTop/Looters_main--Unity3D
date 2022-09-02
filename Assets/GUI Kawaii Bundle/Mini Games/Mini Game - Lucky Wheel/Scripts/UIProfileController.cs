using UnityEngine;

namespace FortuneWheel
{
    public class UIProfileController : MonoBehaviour
    {
        public enum RewardEnum                  // Your Custom Reward Category
        {
            None,
            Gold,
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
            for (int i = 0; i < GameController.ins.PiecesOfWheel.Length; i++)
            {
                GameController.RewardEnum collectedRewardType = GameController.ins.PiecesOfWheel[i].rewardCategory;

                switch (collectedRewardType)
                {
                    case GameController.RewardEnum.Gold:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    case GameController.RewardEnum.Energy:
                        {
                            FindThatElement(RewardEnum.Energy);
                        }
                        break;
                    case GameController.RewardEnum.Life:
                        {
                            FindThatElement(RewardEnum.Life);
                        }
                        break;
                    case GameController.RewardEnum.Money:
                        {
                            FindThatElement(RewardEnum.Money);
                        }
                        break;
                    case GameController.RewardEnum.Gem1:
                        {
                            FindThatElement(RewardEnum.Gem1);
                        }
                        break;
                    case GameController.RewardEnum.Gem2:
                        {
                            FindThatElement(RewardEnum.Gem2);
                        }
                        break;
                    case GameController.RewardEnum.Gem3:
                        {
                            FindThatElement(RewardEnum.Gem3);
                        }
                        break;
                    case GameController.RewardEnum.Gem4:
                        {
                            FindThatElement(RewardEnum.Gem4);
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