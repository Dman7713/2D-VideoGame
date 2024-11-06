using UnityEngine;

public class StartMenuScript : MonoBehaviour
{
    // Reference to the Start Menu UI Panel
    [SerializeField] private GameObject startMenuUI;

    // Method to start the game
    public void StartGame()
    {
        // Hide the Start Menu UI
        startMenuUI.SetActive(false);
        // Enable gameplay elements, if needed
        EnableGameplay();
    }

    // Method for options (optional)
    public void OpenOptions()
    {
        Debug.Log("Options button clicked!");
        // Here you can open options menu UI or settings
    }

    // Method to quit the game
    public void QuitGame()
    {
        Debug.Log("Quit button clicked!");
        Application.Quit();
    }

    // Method to enable gameplay elements
    private void EnableGameplay()
    {
        // Enable any gameplay-specific objects or scripts here
        // Example: gameManager.SetActive(true); or playerController.enabled = true;
    }
}
