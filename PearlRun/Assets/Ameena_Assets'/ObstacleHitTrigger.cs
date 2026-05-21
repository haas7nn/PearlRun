using UnityEngine;

public class ObstacleHitTrigger : MonoBehaviour
{
    public EnemyChase enemyChase;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
        {
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
}