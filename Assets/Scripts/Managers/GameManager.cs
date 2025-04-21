using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Setting")]
    public float gameDuration = 100f;
    public float currentGameTime;
    private bool isGameOver = false;
    private bool isGameStarted = true;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public string mainMenuScene = "StartScene";

    [Header("Player")]
    public PlayerStats playerStats;

    [Header("Spawner")]
    public EnemySpawner enemySpawner;

    private int currentScore = 0;

    private void Start()
    {
        currentGameTime = gameDuration;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        StartGame();
    }

    private void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            currentGameTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentGameTime <= 0 || (playerStats != null && playerStats.currentHealth <= 0))
            {
                GameOver(currentGameTime <= 0);
            }
        }
    }

    void StartGame()
    {
        isGameStarted = true;
        Time.timeScale = 1;

        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentGameTime).ToString();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public void GameOver(bool isVictory)
    {
        isGameOver = true;
        PauseGame();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic();
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = isVictory ?
                    "It's Dawn...\nTotal Score: " + currentScore :
                    "Falling into the eternal sleep\nTotal Score: " + currentScore;
            }
        }

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("PauseMenu");
            AudioManager.Instance.PauseBackgroundMusic();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound("ResumeGame");
            AudioManager.Instance.ResumeBackgroundMusic();
        }
    }

    public void RestartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StartBackgroundMusic();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}