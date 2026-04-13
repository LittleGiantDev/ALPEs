using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform player;
    [SerializeField] private LevelChunk startingChunkPrefab;
    [SerializeField] private LevelChunk[] chunkPrefabs;
    
    [Header("Settings")]
    [SerializeField] private float distanceToSpawnAhead = 60f;
    [SerializeField] private float despawnDistance = 30f;

    private Queue<LevelChunk> activeChunks = new Queue<LevelChunk>();
    private Vector3 spawnPosition = Vector3.zero;
    private int lastChunkIndex = -1;

    private void Start()
    {
        LevelChunk startChunkClone = Instantiate(startingChunkPrefab, spawnPosition, Quaternion.identity);
        activeChunks.Enqueue(startChunkClone);
        spawnPosition = startChunkClone.EndPosition;
    }

    private void Update()
    {
        while (spawnPosition.x - player.position.x < distanceToSpawnAhead)
        {
            SpawnChunk();
        }

        if (activeChunks.Count > 0 && player.position.x > activeChunks.Peek().EndPosition.x + despawnDistance)
        {
            DespawnChunk();
        }
    }

    private void SpawnChunk()
    {
        int randomIndex;
        
        if (chunkPrefabs.Length > 1)
        {
            do
            {
                randomIndex = Random.Range(0, chunkPrefabs.Length);
            } 
            while (randomIndex == lastChunkIndex);
        }
        else
        {
            randomIndex = 0;
        }

        lastChunkIndex = randomIndex;

        LevelChunk newChunk = Instantiate(chunkPrefabs[randomIndex], spawnPosition, Quaternion.identity);
        
        spawnPosition = newChunk.EndPosition;
        activeChunks.Enqueue(newChunk);
    }

    private void DespawnChunk()
    {
        LevelChunk oldChunk = activeChunks.Dequeue();
        Destroy(oldChunk.gameObject);
    }
}