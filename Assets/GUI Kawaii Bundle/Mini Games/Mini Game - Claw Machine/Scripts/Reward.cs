using UnityEngine;

namespace ClawMachine
{
    public class Reward : MonoBehaviour
    {
        [HideInInspector]
        public GameController.RewardEnum rewardType;

        private bool collisitonDetected;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (GameController.ins.RewardsCollected && !collisitonDetected)
            {
                if (collision.transform.tag == "clawInside")
                {
                    collisitonDetected = true;

                    for (int r = 0; r < GameController.ins.Rewards.Length; r++)
                    {
                        if (rewardType == GameController.ins.Rewards[r].rewardType)
                        {
                            GameController.ins.Rewards[r].rewardAmount += 1;
                        }
                    }
                }
            }
        }
    }
}