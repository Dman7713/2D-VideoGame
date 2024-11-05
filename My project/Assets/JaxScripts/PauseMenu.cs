using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseMenuCanvas; // Assign this in the Inspector

    void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = false;
        }
    }

    void Update()
    {
        // Toggle pause menu on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                ShowPauseMenu();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void ShowPauseMenu()
    {
        // Display the pause menu and pause the game
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = true;
            Time.timeScale = 0;
        }
    }

    public void ResumeGame()
    {
        // Hide the pause menu and resume the game
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.enabled = false;
            Time.timeScale = 1;
        }
    }

    public void ResetGame()
    {
        // Reset the game by reloading the current scene
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Quit the game (will only work in a build)
        Time.timeScale = 1;
        Application.Quit();
    }
}
