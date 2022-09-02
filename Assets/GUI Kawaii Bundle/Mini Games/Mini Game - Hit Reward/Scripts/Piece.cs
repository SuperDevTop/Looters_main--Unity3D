using UnityEngine;
using UnityEngine.UI;

namespace HitReward
{
    public class Piece : MonoBehaviour
    {
        public Image backgroundImage;
        public Image rewardIcon;
        public Text rewardAmount;
        public GameController.RewardEnum rewardCategory;

        public int index;

        public void SetValues(int pieceNo)
        {
            index = pieceNo;

            if (GameController.ins.useCustomBackgrounds)
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = GameController.ins.CustomBackgrounds[pieceNo];
            }
            else
            {
                backgroundImage.color = GameController.ins.Rewards[pieceNo].backgroundColor;
                backgroundImage.sprite = GameController.ins.Rewards[pieceNo].backgroundSprite;
            }

            rewardCategory = GameController.ins.Rewards[pieceNo].rewardCategory;
            rewardAmount.text = GameController.ins.Rewards[pieceNo].rewardAmount.ToString();

            for (int i = 0; i < GameController.ins.rewardCategoryIcons.Length; i++)
            {
                if (rewardCategory == GameController.ins.rewardCategoryIcons[i].category)
                {
                    rewardIcon.sprite = GameController.ins.rewardCategoryIcons[i].rewardIcon;
                    GameController.ins.Rewards[pieceNo].rewardIcon = GameController.ins.rewardCategoryIcons[i].rewardIcon;
                }
            }
        }
    }
}