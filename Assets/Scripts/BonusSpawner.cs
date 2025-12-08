using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] bonusPrefabs;
    public float spawnInterval = 8f;
    public float spawnDistance = 40f;
    public float laneDistance = 2f;
    public int lanes = 3;
    
    [Header("Collision Check")]
    public float checkRadius = 1.2f;
    public LayerMask obstacleLayer;

    float timer = 0f;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (obstacleLayer == 0)
        {
            Debug.LogWarning("BonusSpawner: obstacleLayer not set. Using tag check instead.");
        }
    }

    void Update()
    {
        if (player == null) return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        if (bonusPrefabs == null || bonusPrefabs.Length == 0) return;
        
        int lane = Random.Range(0, lanes);
        float x = (lane - (lanes - 1) / 2.0f) * laneDistance;
        float z = player.position.z + spawnDistance;
        Vector3 pos = new Vector3(x, 0.6f, z);
        
        if (IsPositionBlocked(pos))
        {
            for (int i = 0; i < lanes; i++)
            {
                int newLane = (lane + i) % lanes;
                float newX = (newLane - (lanes - 1) / 2.0f) * laneDistance;
                Vector3 newPos = new Vector3(newX, 0.6f, z);
                
                if (!IsPositionBlocked(newPos))
                {
                    pos = newPos;
                    break;
                }
            }
            
            if (IsPositionBlocked(pos))
            {
                Debug.Log("BonusSpawner: All lanes blocked, skipping spawn");
                return;
            }
        }
        
        GameObject prefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
        Instantiate(prefab, pos, prefab.transform.rotation);
    }
    
    bool IsPositionBlocked(Vector3 position)
    {
        if (obstacleLayer != 0)
        {
            Collider[] hits = Physics.OverlapSphere(position, checkRadius, obstacleLayer);
            return hits.Length > 0;
        }
        
        Collider[] allHits = Physics.OverlapSphere(position, checkRadius);
        foreach (var hit in allHits)
        {
            if (hit.CompareTag("Obstacle") || hit.CompareTag("Bonus"))
            {
                return true;
            }
        }
        
        return false;
    }
}