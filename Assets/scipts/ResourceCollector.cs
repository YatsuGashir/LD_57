using UnityEngine;
using System.Collections;

public class ResourceCollector : MonoBehaviour
{
    [Header("Настройки сбора")]
    [SerializeField] private float collectionRadius = 2f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeIntensity = 0.2f;
    [SerializeField] private float fadeDuration = 0.5f;
    private float ironSum = 0f;

    private void Update()
    {
        CheckForOre();
    }

    private void CheckForOre()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, collectionRadius);
        
        foreach (Collider2D collider in hitColliders)
        {
            if (collider != null && collider.CompareTag("Ore") && collider.gameObject.activeInHierarchy)
            {
                StartCoroutine(CollectOre(collider.gameObject));
                break;
            }
        }
    }

    private IEnumerator CollectOre(GameObject ore)
    {
        if (ore == null) yield break;

        // Получаем компоненты руды
        SpriteRenderer oreRenderer = ore.GetComponent<SpriteRenderer>();
        Transform oreTransform = ore.transform;
        Vector3 originalPosition = oreTransform.position;

        // Эффект тряски
        float elapsed = 0f;
        while (elapsed < shakeDuration && ore != null)
        {
            elapsed += Time.deltaTime;
            
            // Плавное уменьшение интенсивности
            float currentShake = shakeIntensity * (1 - elapsed/shakeDuration);
            
            // Применяем тряску к руде
            if (oreTransform != null)
            {
                oreTransform.position = originalPosition + 
                    new Vector3(
                        Random.Range(-currentShake, currentShake),
                        Random.Range(-currentShake, currentShake),
                        0
                    );
            }

            yield return null;
        }

        // Возвращаем руду на место перед исчезновением (если она еще существует)
        if (oreTransform != null)
        {
            oreTransform.position = originalPosition;
        }

        // Эффект исчезновения (если есть SpriteRenderer и объект еще существует)
        if (oreRenderer != null && ore != null)
        {
            elapsed = 0f;
            Color originalColor = oreRenderer.color;
            
            while (elapsed < fadeDuration && ore != null)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed/fadeDuration);
                oreRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }

        // Уничтожаем руду только если она еще существует
        if (ore != null)
        {
            DroneHUD.instance.ironSum += 1;
            DroneHUD.instance.ResourcesUpdate();
            Destroy(ore);
        }
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collectionRadius);
    }
}