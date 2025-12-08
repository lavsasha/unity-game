using UnityEngine;
using TMPro;
using System.Collections;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;

    [Header("Speed blink settings")]
    public Color blinkColor = Color.yellow;
    public float blinkDuration = 0.7f;   
    public int blinkFlashes = 4;  

    Coroutine speedBlinkCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

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
}
