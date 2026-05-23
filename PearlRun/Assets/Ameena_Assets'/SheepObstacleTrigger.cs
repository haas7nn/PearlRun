using UnityEngine;
using System.Collections;
using Ursaanimation.CubicFarmAnimals;

public class SheepObstacleTrigger : MonoBehaviour
{
    public SheepMoveObstacle[] sheepObstacles;
    public float delayBetweenSheep = 0.4f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(StartSheepSequence());
        }
    }

    private IEnumerator StartSheepSequence()
    {
        foreach (SheepMoveObstacle sheep in sheepObstacles)
        {
            if (sheep != null)
            {
                sheep.StartSheepRun();
                yield return new WaitForSeconds(delayBetweenSheep);
            }
        }
    }
}