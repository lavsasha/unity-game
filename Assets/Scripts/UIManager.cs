using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button menuButton;

    [Header("Speed blink settings")]
    public Color blinkColor = Color.yellow;
    public float blinkDuration = 0.7f;   
    public int blinkFlashes = 4;  

    private bool isPaused = false;
    Coroutine speedBlinkCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

        InitializeScoreManager();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void InitializeScoreManager()
    {
        if (ScoreManager.Instance == null)
        {
            GameObject scoreObj = new GameObject("ScoreManager");
            scoreObj.AddComponent<ScoreManager>();
            DontDestroyOnLoad(scoreObj);
        }
        
        if (ScoreManager.Instance != null && scoreText != null && bestScoreText != null)
        {
            ScoreManager.Instance.SetUIReferences(scoreText, bestScoreText);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    #region HUD Updates
    public void UpdateHealth(int cur, int max)
    {
        if (healthText != null) healthText.text = $"HP: {cur}/{max}";
    }

    public void UpdateSpeed(float speed)
    {
        if (speedText != null)
            speedText.text = $"Speed: {speed.ToString("F1", CultureInfo.InvariantCulture)}";
    }

    public void BlinkSpeed()
    {
        if (speedText == null) return;

        if (speedBlinkCoroutine != null) StopCoroutine(speedBlinkCoroutine);
        speedBlinkCoroutine = StartCoroutine(BlinkSpeedCoroutine());
    }

    IEnumerator BlinkSpeedCoroutine()
    {
        Color original = speedText.color;
        float singleFlash = blinkDuration / (float)blinkFlashes;
        for (int i = 0; i < blinkFlashes; i++)
        {
            speedText.color = blinkColor;
            yield return new WaitForSeconds(singleFlash * 0.5f);
            speedText.color = original;
            yield return new WaitForSeconds(singleFlash * 0.5f);
        }
        speedText.color = original;
        speedBlinkCoroutine = null;
    }
    #endregion

    #region Pause Menu
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ReturnToMenu()
    {
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }
    #endregion
}