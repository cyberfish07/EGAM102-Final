using System.Collections;
using UnityEngine;

public class RangedEnemyMovement : MonoBehaviour
{
    Transform player;
    public float moveSpeed = 1.5f;
    public float attackRange = 5f;
    public float retreatRange = 3f;
    public float attackCooldown = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    private float attackTimer;
    private bool isRetreating = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
        // cd
        attackTimer = Random.Range(0f, attackCooldown);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange && attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }
        else if (distanceToPlayer < retreatRange)
        {
            isRetreating = true;
            Vector2 retreatDirection = (transform.position - player.position).normalized;
            transform.position += (Vector3)retreatDirection * moveSpeed * Time.deltaTime;
        }
        else if (distanceToPlayer > attackRange || isRetreating)
        {
            isRetreating = false;
            Vector2 moveDirection = (player.position - transform.position).normalized;
            transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 direction = (player.position - transform.position).normalized;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Face move
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        // SFX
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("EnemyAttack");
    }
}