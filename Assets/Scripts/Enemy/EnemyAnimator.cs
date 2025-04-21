using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector3 previousPosition;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 movementDirection = transform.position - previousPosition;
        if (Mathf.Abs(movementDirection.x) > 0.01f)
        {
            spriteRenderer.flipX = movementDirection.x < -0;
        }

        previousPosition = transform.position;
    }
}