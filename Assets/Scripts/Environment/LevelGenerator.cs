using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform player;
    [SerializeField] private LevelChunk startingChunkPrefab;
    [SerializeField] private LevelChunk[] chunkPrefabs;
    [SerializeField] private LevelChunk tavernChunkPrefab;
    
    [Header("Settings")]
    [SerializeField] private float distanceToSpawnAhead = 60f;
    [SerializeField] private float despawnDistance = 30f;
    [SerializeField] private float tavernInterval = 1000f;

    private List<LevelChunk> activeChunks = new List<LevelChunk>();
    private Vector3 spawnPosition = Vector3.zero;
    private int lastChunkIndex = -1;
    private float nextTavernSpawnX;

    private void Start()
    {
        nextTavernSpawnX = tavernInterval;
        
        LevelChunk startChunkClone = Instantiate(startingChunkPrefab, spawnPosition, Quaternion.identity);
        activeChunks.Add(startChunkClone);
        spawnPosition = startChunkClone.EndPosition;
    }

    //Comprueba de forma continua si hay que generar terreno por delante o borrar el que ha quedado atrás
    private void Update()
    {
        while (spawnPosition.x - player.position.x < distanceToSpawnAhead)
        {
            SpawnChunk();
        }

        if (activeChunks.Count > 0 && player.position.x > activeChunks[0].EndPosition.x + despawnDistance)
        {
            DespawnChunk();
        }
    }

    //Elige y crea el siguiente bloque del nivel
    private void SpawnChunk()
    {
        LevelChunk prefabToSpawn;

        if (spawnPosition.x >= nextTavernSpawnX)
        {
            prefabToSpawn = tavernChunkPrefab;
            nextTavernSpawnX += tavernInterval;
            lastChunkIndex = -1; 
        }
        else
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
            prefabToSpawn = chunkPrefabs[randomIndex];
        }

        LevelChunk newChunk = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        
        spawnPosition = newChunk.EndPosition;
        activeChunks.Add(newChunk);
    }

    // Elimina el bloque del nivel más antiguo de nuestra lista
    private void DespawnChunk()
    {
        LevelChunk oldChunk = activeChunks[0];
        activeChunks.RemoveAt(0);
        Destroy(oldChunk.gameObject);
    }
}