using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject[] obstaclePrefabs;

    public float spawnInterval = 1f;
    public float spawnDistance = 50f;

    public float laneDistance = 2f;
    public int lanes = 3;

    public float minSpacingZ = 6f;
    public float minSpacingRadius = 1.2f;
    
    [Header("Bonus Check")]
    public LayerMask bonusLayer;

    private float[] lastZ;
    private float timer = 0f;

    void Start()
    {
        if (player == null)
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo != null) player = pgo.transform;
        }

        lastZ = new float[Mathf.Max(1, lanes)];
        for (int i = 0; i < lastZ.Length; i++)
            lastZ[i] = -999f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnOne();
            timer = 0f;
        }
    }

    void SpawnOne()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;
        if (player == null) return;

        int lane = Random.Range(0, lanes);
        float baseX = (lane - (lanes - 1) / 2.0f) * laneDistance;
        float z = player.position.z + spawnDistance;

        if (z - lastZ[lane] < minSpacingZ)
            return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        if (prefab == null) return;
        Obstacle cfg = prefab.GetComponent<Obstacle>();

        Vector3 pos = new Vector3(baseX, 0.5f, z);

        if (cfg != null)
            pos += cfg.spawnOffset;
        Collider[] hits = Physics.OverlapSphere(pos, minSpacingRadius);
        foreach (var h in hits)
        {
            if (h.CompareTag("Obstacle") || h.CompareTag("Bonus"))
                return;
        }

        Quaternion rot = (cfg != null) ? Quaternion.Euler(cfg.spawnEuler) : Quaternion.identity;

        GameObject obj = Instantiate(prefab, pos, rot);
        obj.tag = "Obstacle";

        lastZ[lane] = z;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.cyan;
        float x = (0 - (lanes - 1) / 2.0f) * laneDistance;
        float z = player != null ? player.position.z + spawnDistance : spawnDistance;
        Vector3 pos = new Vector3(x, 0.5f, z);
        Gizmos.DrawWireSphere(pos, minSpacingRadius);
    }
#endif
}