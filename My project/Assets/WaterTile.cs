using UnityEngine;

public class WaterTile : MonoBehaviour
{
    public float fallSpeed = 2.0f;    // Speed of falling
    public LayerMask groundLayer;     // The ground layer to check for collisions

    private bool isFalling = true;

    void Update()
    {
        if (isFalling)
        {
            Vector2 belowPosition = new Vector2(transform.position.x, transform.position.y - 1);

            if (!Physics2D.OverlapPoint(belowPosition, groundLayer))
            {
                transform.position = Vector2.MoveTowards(transform.position, belowPosition, fallSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 leftPosition = new Vector2(transform.position.x - 1, transform.position.y);
                Vector2 rightPosition = new Vector2(transform.position.x + 1, transform.position.y);

                bool canMoveLeft = !Physics2D.OverlapPoint(leftPosition, groundLayer);
                bool canMoveRight = !Physics2D.OverlapPoint(rightPosition, groundLayer);

                if (canMoveLeft)
                {
                    transform.position = leftPosition;
                }
                else if (canMoveRight)
                {
                    transform.position = rightPosition;
                }
                else
                {
                    isFalling = false; // Stop moving if no space is available
                }
            }
        }
    }
}
