using UnityEngine;
using System.Collections;

public class GameSpeedManager : MonoBehaviour
{
    public static GameSpeedManager Instance { get; private set; }

    [Header("Speed settings")]
    public float startSpeed = 12f;     
    public float stepAmount = 1f;  
    public float stepInterval = 10f; 
    public float maxSpeed = 30f; 

    [Header("UI")]
    public bool updateUiOnStart = true;

    float currentSpeed;
    float timer = 0f;

    public float CurrentSpeed => currentSpeed;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        currentSpeed = startSpeed;
        timer = 0f;

        if (updateUiOnStart && UIManager.Instance != null)
            UIManager.Instance.UpdateSpeed(currentSpeed);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= stepInterval)
        {
            timer -= stepInterval;
            TryIncreaseSpeed();
        }
    }

    void TryIncreaseSpeed()
    {
        if (currentSpeed >= maxSpeed) return;

        currentSpeed = Mathf.Min(currentSpeed + stepAmount, maxSpeed);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSpeed(currentSpeed);
            UIManager.Instance.BlinkSpeed();
        }
    }

    public void IncreaseSpeedImmediate(float amount)
    {
        currentSpeed = Mathf.Min(currentSpeed + amount, maxSpeed);
        timer = 0f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateSpeed(currentSpeed);
            UIManager.Instance.BlinkSpeed();
        }
    }

    public void ResetSpeed()
    {
        currentSpeed = startSpeed;
        timer = 0f;
        if (UIManager.Instance != null) UIManager.Instance.UpdateSpeed(currentSpeed);
    }
}
