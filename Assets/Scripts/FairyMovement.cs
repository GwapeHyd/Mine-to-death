using UnityEngine;

public class FairyMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float changeDirectionInterval = 3f;
    private Vector2 movementDirection= Vector2.right;
    private float directionChangeTimer;
    private void Awake()
    {
        directionChangeTimer = changeDirectionInterval;
        ChooseNewDirection();
    }

    private void Update()
    {
        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0f)
        {
            ChooseNewDirection();
            directionChangeTimer = changeDirectionInterval;
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(movementDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void ChooseNewDirection()
    {
        movementDirection = -movementDirection;
    }

    private void LateUpdate()
    {
        float newY = transform.position.y + Mathf.Sin(Time.time * 2f) * 0.005f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    
}
