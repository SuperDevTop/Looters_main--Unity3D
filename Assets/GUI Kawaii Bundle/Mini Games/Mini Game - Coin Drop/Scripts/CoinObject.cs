using UnityEngine;
namespace DropGame
{
    public class CoinObject : MonoBehaviour
    {
        private bool collisitonDetected;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collisitonDetected)
            {
                switch (collision.transform.tag)
                {
                    case "Slot01":
                        {
                            SetRewardIndex(0);
                            break;
                        }
                    case "Slot02":
                        {
                            SetRewardIndex(1);
                            break;
                        }
                    case "Slot03":
                        {
                            SetRewardIndex(2);
                            break;
                        }
                    case "Slot04":
                        {
                            SetRewardIndex(3);
                            break;
                        }
                    case "Slot05":
                        {
                            SetRewardIndex(4);
                            break;
                        }
                }
            }
        }

        private void SetRewardIndex(int index)
        {
            collisitonDetected = true;
            GameController.ins.CollectedRewardIndex = index;
            GameController.ins.RewardResults();
        }
    }
}