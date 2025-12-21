using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("Settings")]
    public float timeScaleOnGameOver = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else 
            Instance = this;
    }

    void Start()
    {
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
        
        Time.timeScale = 1f;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScoreForNewGame();
        }
        else
        {
            Debug.LogWarning("GameManager: ScoreManager.Instance is null!");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPlayerDeath()
    {
        ShowGameOver();
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null && ScoreManager.Instance != null)
            {
                int finalScore = ScoreManager.Instance.GetCurrentScore();
                finalScoreText.text = $"Score: {finalScore}";
            }
            
            if (bestScoreText != null && ScoreManager.Instance != null)
            {
                ScoreManager.Instance.UpdateBestScore();
                int bestScore = ScoreManager.Instance.GetBestScore();
                bestScoreText.text = $"Best: {bestScore}";
            }
        }
        
        Time.timeScale = timeScaleOnGameOver;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        if (gameOverPanel != null) 
            gameOverPanel.SetActive(false);
        
        Time.timeScale = 1f;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScoreForNewGame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}