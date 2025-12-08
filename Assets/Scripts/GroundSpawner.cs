using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject groundTilePrefab;

    [Header("Settings")]
    public int initialTiles = 5;    
    public float tileLength = 30f;  
    public int tilesAhead = 3;     
    public int tilesBehind = 2;  

    private LinkedList<GameObject> activeTiles = new LinkedList<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        for (int i = 0; i < initialTiles; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        if (player == null) return;
        float playerZ = player.position.z;
        while (spawnZ - playerZ < tilesAhead * tileLength)
        {
            SpawnTile();
        }
        while (activeTiles.Count > 0)
        {
            GameObject first = activeTiles.First.Value;
            if (playerZ - first.transform.position.z > (tilesBehind + 0.5f) * tileLength)
            {
                Destroy(first);
                activeTiles.RemoveFirst();
            }
            else break;
        }
    }

    void SpawnTile()
    {
        if (groundTilePrefab == null)
        {
            Debug.LogWarning("GroundSpawner: groundTilePrefab not set");
            return;
        }

        Vector3 pos = new Vector3(0f, 0f, spawnZ);
        GameObject tile = Instantiate(groundTilePrefab, pos, Quaternion.identity);
        tile.name = "GroundTile_" + spawnZ;
        activeTiles.AddLast(tile);
        spawnZ += tileLength;
    }
}
