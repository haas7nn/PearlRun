using UnityEngine;

public class Level5AmwajObstacleHitTrigger : MonoBehaviour
{
    public Level5AmwajEnemyChase enemyChase;

    private bool hasHitPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasHitPlayer)
            return;

        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            hasHitPlayer = true;

            if (Level5AmwajRunnerGameManager.instance != null)
            {
                Level5AmwajRunnerGameManager.instance.PlayerHit();
                Debug.Log("Player hit obstacle. Life decreased.");
            }
            else
            {
                Debug.LogWarning("Level5AmwajRunnerGameManager instance was not found.");
            }

            if (enemyChase != null)
            {
                enemyChase.ForceChaseFromBehind();
                Debug.Log("Enemy appeared behind player.");
            }
            else
            {
                Debug.LogWarning("Level5AmwajEnemyChase is not assigned on this obstacle.");
            }
        }
    }

    public void ResetHit()
    {
        hasHitPlayer = false;
    }
}