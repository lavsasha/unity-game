using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Time-based Score Settings")]
    public float scoreInterval = 15f;
    public int scorePerInterval = 10;

    [Header("Bonus Score Settings")]
    public int healBonusScore = 5;
    public int invulnerabilityBonusScore = 8;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    private int currentScore = 0;
    private int bestScore = 0;
    private float timer = 0f;
    private string bestScoreKey = "BestScore";

    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnBestScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject);
            return;
        }
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadBestScore();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= scoreInterval)
        {
            AddScore(scorePerInterval);
            timer = 0f;
        }
    }

    void LoadBestScore()
    {
        bestScore = PlayerPrefs.GetInt(bestScoreKey, 0);
    }

    void SaveBestScore()
    {
        PlayerPrefs.SetInt(bestScoreKey, bestScore);
        PlayerPrefs.Save();
    }

    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        int oldScore = currentScore;
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
    }

    public void UpdateBestScore() {
        if (currentScore > bestScore)
        {
            int oldBest = bestScore;
            bestScore = currentScore;
            SaveBestScore();
            OnBestScoreChanged?.Invoke(bestScore);
            UpdateBestScoreUI();
        }
    }

    public void AddHealBonusScore()
    {
        AddScore(healBonusScore);
    }

    public void AddInvulnerabilityBonusScore()
    {
        AddScore(invulnerabilityBonusScore);
    }

    public void ResetScoreForNewGame()
    {
        currentScore = 0;
        timer = 0f;
        OnScoreChanged?.Invoke(currentScore);
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        ResetScoreForNewGame();
    }

    public int GetCurrentScore() => currentScore;
    public int GetBestScore() => bestScore;

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = $"Best: {bestScore}";
        }
    }

    void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateBestScoreUI();
    }

    public void SetUIReferences(TextMeshProUGUI newScoreText, TextMeshProUGUI newBestScoreText)
    {
        scoreText = newScoreText;
        bestScoreText = newBestScoreText;
        UpdateAllUI();
    }
}