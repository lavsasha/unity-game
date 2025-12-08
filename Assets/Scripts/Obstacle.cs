using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int damage = 1;

    [Header("Spawner Settings")]
    public Vector3 spawnOffset = Vector3.zero;
    public Vector3 spawnEuler = Vector3.zero;

    [Header("Cleanup")]
    public float despawnZOffset = -15f;

    private bool wasHit = false;
    private Transform playerTransform;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;
        if (transform.position.z < playerTransform.position.z + despawnZOffset)
        {
            Destroy(gameObject);
        }
    }
    
    public bool TryHit()
    {
        if (wasHit) return false;
        wasHit = true;

        Destroy(gameObject);
        return true;
    }
}
