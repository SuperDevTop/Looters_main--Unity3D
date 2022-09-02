using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SlotMachine
{
    public class Column : MonoBehaviour
    {
        public Image[] rowImages;

        public int currentSlot;
        public int minShiftCount, maxShiftCount;

        private int cellCount;           //Difference next slot and current slot
        private int randomValue;         //How many times rows will be shifted untill result value.
        private int randomShiftCount;    //How many row to shift

        private float timeInternal;
        private static float yValuePerRow = 150f;

        private bool columnStopped = true;

        private void Start()
        {
            for (int i = 0; i < GameController.ins.SlotTypes.Length; i++)
            {
                rowImages[i].sprite = GameController.ins.SlotTypes[i].slotIcon; //They should be equal length
            }
        }

        public bool ColumnStopped
        {
            get
            {
                return columnStopped;
            }
        }

        public void StartRotating()
        {
            cellCount = 0;
            randomShiftCount = Random.Range(minShiftCount, maxShiftCount);
            StartCoroutine(RotateRoutine());
        }

        private IEnumerator RotateRoutine()
        {
            columnStopped = false;
            timeInternal = 0.1f;

            if (GameController.ins.NextSlotSelected)
            {
                cellCount = (GameController.ins.SlotTypes.Length + (GameController.ins.NextSlotIndex - currentSlot));

                randomValue = (randomShiftCount - (randomShiftCount % GameController.ins.SlotTypes.Length)) + cellCount;
            }
            else
                randomValue = Random.Range(minShiftCount, maxShiftCount);

            for (int i = 0; i < randomValue; i++)
            {
                float slotIndex = Mathf.Abs(transform.localPosition.y / yValuePerRow);

                if (slotIndex == 6) //rowCount-1
                {
                    float yPosition = 0 - (slotIndex * yValuePerRow); //-960 for our case
                    transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - yPosition); //Back to starting position
                }
                else
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - yValuePerRow);
                }
                yield return new WaitForSeconds(timeInternal);
            }

            currentSlot = -Mathf.RoundToInt(transform.localPosition.y / yValuePerRow);
            columnStopped = true;
        }
    }
}

// 0, -160, -320, -480, -640, -800, -960