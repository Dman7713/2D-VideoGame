using UnityEngine;

public class FlipSpriteOnMouseSide : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get the midpoint of the screen width
        float screenMidpoint = Screen.width / 2;

        // Flip the sprite on the y-axis if the mouse is on the left side, reset it if on the right side
        spriteRenderer.flipY = Input.mousePosition.x < screenMidpoint;
    }
}
