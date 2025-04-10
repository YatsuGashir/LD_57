using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    public Transform platform;
    public SpawnTrigger[] spawnTriggers;
    public GameObject[] enemyPrefabs;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    private void Awake()
    {
        instance = this;
    }

    public void ActivateTrigger(int index)
    {
        if (index < 0 || index >= spawnTriggers.Length) return;

        var trigger = spawnTriggers[index];
        if (trigger.isTriggered) return;

        Debug.Log($"Trigger {index} activated");
        trigger.isTriggered = true;
        StartCoroutine(SpawnEnemies(trigger));
    }
    


    // Управление спавном врагов через триггеры
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("trigger enter");
            foreach (var trigger in spawnTriggers)
            {
                if (!trigger.isTriggered /*&& trigger.triggerCollider.bounds.Contains(other.transform.position))
                {
                    StartCoroutine(SpawnEnemies(trigger));
                    trigger.isTriggered = true; // Отмечаем триггер как активированный
                }
            }
        }
    }*/

    // Короутина для спавна врагов из сценариев
    private IEnumerator SpawnEnemies(SpawnTrigger trigger)
    {
        foreach (var scenario in trigger.spawnScenarios)
        {
            yield return new WaitForSeconds(scenario.delayBeforeStart);

            for (int i = 0; i < scenario.enemyCount; i++)
            {
                GameObject enemyPrefab = enemyPrefabs[scenario.enemyPrefabIndex];
                SpawnEnemy(enemyPrefab);
                yield return new WaitForSeconds(scenario.spawnInterval);
            }

            yield return new WaitForSeconds(scenario.delayBetweenGroups);
        }
    }

    // Метод для спавна врага
    void SpawnEnemy(GameObject enemyPrefab)
    {
        // Получаем позицию платформы
        Vector2 platformPosition = platform.position;

        // Генерируем случайную позицию по оси X в пределах заданной области
        float spawnX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);

        // Спавним врага немного выше платформы, добавляя смещение по оси Y
        float spawnY = platformPosition.y + Random.Range(10f, 20f); // Смещение вверх от платформы

        // Создаем врага на вычисленной позиции
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Debug.Log(spawnX + " " + spawnY);
    }
}

[System.Serializable]
public class SpawnTrigger
{
    public Collider2D triggerCollider;       // Триггерная область
    public bool isTriggered = false;         // Флаг, чтобы не спавнить несколько раз
    public SpawnScenario[] spawnScenarios;   // Сценарии спавна для этого триггера
}

[System.Serializable]
public class SpawnScenario
{
    public int enemyPrefabIndex;             // Индекс врага в массиве префабов
    public int enemyCount = 1;               // Количество врагов, которое будет заспавнено
    public float spawnInterval = 1f;         // Интервал между спавнами врагов
    public float delayBeforeStart = 0f;      // Задержка перед началом спавна
    public float delayBetweenGroups = 2f;    // Задержка между группами врагов
}
