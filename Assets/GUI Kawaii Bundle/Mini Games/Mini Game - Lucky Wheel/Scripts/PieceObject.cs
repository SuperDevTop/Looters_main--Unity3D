using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    public class PieceObject : MonoBehaviour
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
                backgroundImage.color = GameController.ins.PiecesOfWheel[pieceNo].backgroundColor;
                backgroundImage.sprite = GameController.ins.PiecesOfWheel[pieceNo].backgroundSprite;
            }

            rewardCategory = GameController.ins.PiecesOfWheel[pieceNo].rewardCategory;
            rewardAmount.text = GameController.ins.PiecesOfWheel[pieceNo].rewardAmount.ToString();

            for (int i = 0; i < GameController.ins.categoryIcons.Length; i++)
            {
                if (rewardCategory == GameController.ins.categoryIcons[i].category)
                {
                    rewardIcon.sprite = GameController.ins.categoryIcons[i].rewardIcon;
                    GameController.ins.PiecesOfWheel[pieceNo].rewardIcon = GameController.ins.categoryIcons[i].rewardIcon;
                }
            }
        }
    }
}