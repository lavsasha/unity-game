using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public MonoBehaviour movementScript;

    private int currentHealth;
    private bool isDead = false;

    [Header("Collision / bounce control")]
    public float maxCollisionBounceY = 4f;

    [Header("Invulnerability visuals")]
    public Color invulnerableColor = new Color(0.2f, 0.9f, 1f, 1f);

    bool isInvulnerable = false;
    Coroutine invulCoroutine = null;
    Renderer cachedRenderer;
    Color originalColor;

    void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
        if (cachedRenderer != null) originalColor = cachedRenderer.material.color;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance?.UpdateHealth(currentHealth, maxHealth);
    }

    public void ApplyDamage(int amount)
    {
        if (isDead) return;
        if (isInvulnerable) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UIManager.Instance?.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UIManager.Instance?.UpdateHealth(currentHealth, maxHealth);
    }

    public void MakeInvulnerable(float seconds)
    {
        if (invulCoroutine != null)
            StopCoroutine(invulCoroutine);
        invulCoroutine = StartCoroutine(InvulnerabilityRoutine(seconds));
    }

    IEnumerator InvulnerabilityRoutine(float seconds)
    {
        isInvulnerable = true;
        if (cachedRenderer != null)
            cachedRenderer.material.color = invulnerableColor;

        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
        if (cachedRenderer != null)
            cachedRenderer.material.color = originalColor;

        invulCoroutine = null;
    }

    void Die()
    {
        isDead = true;
        if (movementScript != null)
            movementScript.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        GameManager.Instance?.OnPlayerDeath();
    }

    void OnCollisionEnter(Collision collision)
    {
        Obstacle obstacle = collision.collider.GetComponentInParent<Obstacle>();
        if (obstacle != null)
        {
            if (obstacle.TryHit())
            {
                ApplyDamage(obstacle.damage);
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    bool nonGroundContact = false;
                    foreach (var c in collision.contacts)
                    {
                        if (Vector3.Dot(c.normal, Vector3.up) < 0.5f)
                        {
                            nonGroundContact = true;
                            break;
                        }
                    }

                    if (nonGroundContact)
                    {
                        Vector3 vel = rb.linearVelocity;
                        if (vel.y > maxCollisionBounceY)
                        {
                            vel.y = maxCollisionBounceY;
                            rb.linearVelocity = vel;
                        }
                    }
                }
            }
        }
    }
}
