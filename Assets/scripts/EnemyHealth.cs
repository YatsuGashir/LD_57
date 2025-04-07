using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;
    public string enemyTag = "Enemy";
    [Header("Смерть")]
    [SerializeField] private ParticleSystem deathParticle;

    void Start()
    {
        deathParticle.Play();
        deathParticle.Stop();
        if (deathParticle != null)
            deathParticle.Stop();
        
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        int randomChoice = Random.Range(0, 2);
        if (randomChoice == 0)
        {
                AudioManager.instance.Play("enemydeath1");
        }
        else
        {
                AudioManager.instance.Play("enemydeath2");
        }

        StartCoroutine(DieParticle());
    }

    private IEnumerator DieParticle()
    {
            deathParticle.Play();
            yield return new WaitForSeconds(1f); // Исправлено: добавлено 'new'
        
        Destroy(gameObject); // Перенесено в корутину после проигрывания эффекта
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
        }
    }
}