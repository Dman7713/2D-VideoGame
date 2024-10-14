using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GridSystem gridSystem;

    public ParticleType currentParticle = ParticleType.Sand;

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left-click to add particle
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mousePos.x / gridSystem.cellSize);
            int y = Mathf.FloorToInt(mousePos.y / gridSystem.cellSize);

            gridSystem.AddParticle(currentParticle, x, y);
        }

        // Toggle between sand and water with the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentParticle = currentParticle == ParticleType.Sand ? ParticleType.Water : ParticleType.Sand;
        }
    }
}
