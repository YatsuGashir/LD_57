using UnityEngine;

public class PlatformHealth : MonoBehaviour
{
    public static System.Action OnPlatformDestroyed; // Событие уничтожения платформы

    public int maxHP = 100;
    public int currentHP;
    [SerializeField] private PlatformBar platformBar;
    [SerializeField] private Animator animator;

    [Header("Улучшения платформы")]
    public PlatformUpgrade[] upgrades;
    public SpriteRenderer[] platformSpriteRenderers;

    private int currentUpgradeIndex = 0;
    private int count=1;

    void Start()
    {
        currentHP = maxHP;
        platformBar.SetMaxBar(maxHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        platformBar.SetBar(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Вызываем событие перед уничтожением
        OnPlatformDestroyed?.Invoke();
        
        Destroy(gameObject);
    }

    public void ApplyUpgrade(int index)
    {
        if (index < 0 || index >= upgrades.Length) return;

        PlatformUpgrade upgrade = upgrades[index];

        // Обновление спрайтов
        for (int i = 0; i < platformSpriteRenderers.Length && i < upgrade.platformSprites.Length; i++)
        {
            if (upgrade.platformSprites[i] != null)
                platformSpriteRenderers[i].sprite = upgrade.platformSprites[i];
        }

        count++;
        string lvl = "LVL" + count; 
        animator.SetTrigger(lvl);
        maxHP += upgrade.additionalHP;
        currentHP += upgrade.additionalHP;
        platformBar.SetMaxBar(maxHP);
        platformBar.SetBar(currentHP);

        Debug.Log("Платформа улучшена: " + upgrade.upgradeName);
    }

    public int GetUpgradeCount() => upgrades.Length;
}