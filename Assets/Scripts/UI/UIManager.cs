using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public Button restartButton;
    public Button pauseButton;
    public Button resumeButton;
    public GameObject pausePanel;

    [Header("Game Manager")]
    public GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<GameManager>();

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartButtonClicked);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pausePanel != null)
        {
            bool isPaused = !pausePanel.activeSelf;
            pausePanel.SetActive(isPaused);

            if (gameManager != null)
            {
                if (isPaused)
                    gameManager.PauseGame();
                else
                    gameManager.ResumeGame();
            }
        }
    }

    public void OnRestartButtonClicked()
    {
        if (gameManager != null)
            gameManager.RestartGame();
    }

    public void OnPauseButtonClicked()
    {
        if (pausePanel != null)
        {
            bool isPaused = !pausePanel.activeSelf;
            pausePanel.SetActive(isPaused);

            if (gameManager != null)
            {
                if (isPaused)
                    gameManager.PauseGame();
                else
                    gameManager.ResumeGame();
            }
        }
    }

    public void OnReturnToMainMenuClicked()
    {
        if (gameManager != null)
            gameManager.ReturnToMainMenu();
    }
}