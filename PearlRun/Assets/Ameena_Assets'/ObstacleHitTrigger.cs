using UnityEngine;

public class ObstacleHitTrigger : MonoBehaviour
{
    public EnemyChase enemyChase;

    private bool hasHitPlayer = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitPlayer)
            return;

        if (collision.gameObject.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
        {
            hasHitPlayer = true;

            // Tell the game manager that the player got hit
            if (Level3RunnerGameManager.instance != null)
            {
                Level3RunnerGameManager.instance.PlayerHit();
            }
            else
            {
                Debug.LogWarning("RunnerGameManager instance was not found.");
            }

            // Make the enemy chase from behind
            if (enemyChase != null)
            {
                enemyChase.ForceChaseFromBehind();
                Debug.Log("Player hit obstacle. Enemy appeared behind player.");
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