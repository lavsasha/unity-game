using UnityEngine;

public enum BonusType
{
    Heal,
    Invulnerability
}

[RequireComponent(typeof(Collider))]
public class BonusPickup : MonoBehaviour
{
    [Header("General")]
    public BonusType bonusType = BonusType.Heal;
    public float lifetime = 30f;
    public string playerTag = "Player";

    [Header("Heal")]
    public int healAmount = 1;

    [Header("Invulnerability")]
    public float invulnerabilitySeconds = 3f;

    void Start()
    {
        if (lifetime > 0f) Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        PlayerHealth ph = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
        if (ph == null)
        {
            Debug.LogWarning("BonusPickup: PlayerHealth not found on colliding object.");
            return;
        }

        switch (bonusType)
        {
            case BonusType.Heal:
                ph.Heal(healAmount);
                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.AddHealBonusScore();
                break;
                
            case BonusType.Invulnerability:
                ph.MakeInvulnerable(invulnerabilitySeconds);
                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.AddInvulnerabilityBonusScore();
                break;
        }

        Destroy(gameObject);
    }
}
