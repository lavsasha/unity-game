using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Components")]
    public Button startButton;
    public Button quitButton;
    public TextMeshProUGUI gameTitleText;
    public TextMeshProUGUI developerText;
    public TextMeshProUGUI bestScoreText;

    [Header("Settings")]
    public string gameSceneName = "GameScene";
    public string gameTitle = "ENDLESS RUNNER";

    void Start()
    {
        if (gameTitleText != null)
            gameTitleText.text = gameTitle;

        UpdateBestScoreDisplay();
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StartGame()
    {
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
            ScoreManager.Instance = null;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void UpdateBestScoreDisplay()
    {
        if (bestScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            bestScoreText.text = $"Best score: {bestScore}";
        }
    }

    void OnEnable()
    {
        UpdateBestScoreDisplay();
    }
}