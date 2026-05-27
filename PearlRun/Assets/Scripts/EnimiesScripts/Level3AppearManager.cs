using UnityEngine;

public class Level3AppearManager : MonoBehaviour
{
    public GameObject enemy;
    public Transform player;

    [Header("Spawn Settings")]
    public float behindDistance = 4f;
    public float heightOffset = 0f;

    public void ShowEnemyBehindPlayer()
    {
        if (enemy == null || player == null) return;

        Vector3 spawnPosition = player.position - player.forward * behindDistance;
        spawnPosition.y = player.position.y + heightOffset;

        enemy.transform.position = spawnPosition;
        enemy.SetActive(true);
    }

    public void HideEnemy()
    {
        if (enemy != null)
        {
            enemy.SetActive(false);
        }
    }
}