using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehaviour : ProjectileWeaponBehaviour
{
    [Header("Effect")]
    public float burnDuration = 3f;
    public int burnDamagePerTick = 5;
    public float burnTickRate = 0.5f;
    public float knockbackForce = 3f;

    public GameObject fireEffect;

    private List<EnemyStats> burnedEnemies = new List<EnemyStats>();

    protected override void Start()
    {
        base.Start();
        burnDuration = 3.0f;
        burnDamagePerTick = 8;
        burnTickRate = 0.5f;
        knockbackForce = 5.0f;
    }


    IEnumerator ApplyBurnEffect(EnemyStats enemy)
    {
        float elapsedTime = 0f;
        float nextTickTime = 0f;

        SpriteRenderer enemySprite = enemy.GetComponent<SpriteRenderer>();
        Color originalColor = enemySprite ? enemySprite.color : Color.white;
        Color burnColor = Color.red;
        float nextBlinkTime = 0f;
        bool isBlinking = false;

        while (elapsedTime < burnDuration && enemy != null)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= nextTickTime)
            {
                nextTickTime = elapsedTime + burnTickRate;
                if (enemy != null)
                {
                    enemy.TakeDamage(burnDamagePerTick);
                }
            }

            if (elapsedTime >= nextBlinkTime && enemySprite != null)
            {
                nextBlinkTime = elapsedTime + 0.1f;
                isBlinking = !isBlinking;
                enemySprite.color = isBlinking ? burnColor : originalColor;
            }

            yield return null;
        }

        if (enemySprite != null)
        {
            enemySprite.color = originalColor;
        }

        if (enemy != null)
        {
            burnedEnemies.Remove(enemy);
        }
    }

    protected override void HandleEnemyCollision(Collider2D collision)
    {
        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        EnemyMovement enemyMovement = collision.GetComponent<EnemyMovement>();

        if (enemy != null)
        {
            if (enemyMovement != null)
            {
                enemyMovement.ApplyKnockback(direction, knockbackForce, 0.3f);
            }

            if (!burnedEnemies.Contains(enemy))
            {
                burnedEnemies.Add(enemy);
                StartCoroutine(ApplyBurnEffect(enemy));

                if (fireEffect != null)
                {
                    GameObject effect = Instantiate(fireEffect, collision.transform);
                    Destroy(effect, burnDuration);
                }
            }

            Destroy(gameObject);
        }
    }
}