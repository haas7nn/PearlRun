using UnityEngine;

public class Level3ObstacleHitTrigger : MonoBehaviour
{
    public Level3EnemyChase enemyChase;

    private bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer)
            return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            hasHitPlayer = true;

            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.PlayerHit();
                Debug.Log("Player hit obstacle. Life decreased.");
            }
            else
            {
                Debug.LogWarning("Level3RunnerGameManager instance was not found.");
            }

            if (enemyChase != null)
            {
                enemyChase.ForceChaseFromBehind();
                Debug.Log("Enemy appeared behind player.");
            }
            else
            {
                Debug.LogWarning("EnemyChase is not assigned on this obstacle.");
            }
        }
    }

    public void ResetHit()
    {
        hasHitPlayer = false;
    }
}