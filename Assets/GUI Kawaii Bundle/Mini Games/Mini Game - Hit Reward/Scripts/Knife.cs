using System.Collections;
using UnityEngine;

namespace HitReward
{
    public class Knife : MonoBehaviour
    {
        private float speed = 0.4f;
        private bool move = true;
         
        public void HitTarget()
        {
            StartCoroutine(MoveRoutine());
        }

        IEnumerator MoveRoutine()
        {
            while (move)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + speed, transform.position.z);
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (move) //Here there is no need to check collision tag because there is no other option rather than a piece of wheel to be collide.
            {
                GameController.ins.PlayHitAnim();
                transform.SetParent(collision.transform.parent); //Be child of collided piece of wheel
                transform.SetSiblingIndex(0);
                move = false;
                StopCoroutine(MoveRoutine());
                GameController.ins.SlowDownWheel();
            }

        }
    }
}