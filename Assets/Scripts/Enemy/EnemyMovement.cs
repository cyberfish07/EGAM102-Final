using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Transform player;
    public float moveSpeed;


    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);    //Constantly move the enemy towards the player
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        StartCoroutine(KnockbackCoroutine(direction, force, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 direction, float force, float duration)
    {
        float originalSpeed = moveSpeed;

        // Prevent against
        moveSpeed = 0;

        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.position += (Vector3)(direction * force * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        moveSpeed = originalSpeed;
    }
}
