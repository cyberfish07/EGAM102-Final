using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [Header("UI")]
    public Button startButton;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}