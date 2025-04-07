using UnityEngine;

public class SpawnTriggerActivator : MonoBehaviour
{
    public int triggerIndex; // индекс триггера в массиве EnemySpawner

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnemySpawner.instance.ActivateTrigger(triggerIndex);
        }
    }
}