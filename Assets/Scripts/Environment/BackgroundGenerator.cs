using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private BackgroundChunk[] chunkPrefabs;
    [SerializeField] private float distanceToSpawn = 60f;
    [SerializeField] private float despawnDistance = 30f;

    private List<BackgroundChunk> activeChunks = new List<BackgroundChunk>();
    private BackgroundChunk backChunk;

    private void Start()
    {
        SpawnInitialChunks();
    }

    //Comprueba si hay que crear terreno nuevo o destruir el que quedó atrás
    private void Update()
    {
        if (activeChunks.Count == 0) return;

        BackgroundChunk lastChunk = activeChunks[activeChunks.Count - 1];

        if (lastChunk.EndPosition.x - cameraTransform.position.x < distanceToSpawn)
        {
            SpawnChunk();
        }

        if (cameraTransform.position.x > activeChunks[0].EndPosition.x + despawnDistance)
        {
            DespawnChunk();
        }

        if (backChunk != null && cameraTransform.position.x > backChunk.EndPosition.x + despawnDistance)
        {
            Destroy(backChunk.gameObject);
            backChunk = null;
        }
    }

    private void SpawnInitialChunks()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        BackgroundChunk startChunk = Instantiate(chunkPrefabs[randomIndex], transform);
        
        Vector3 offset = transform.position - startChunk.StartPosition;
        startChunk.transform.position += offset;
        
        activeChunks.Add(startChunk);

        int backIndex = Random.Range(0, chunkPrefabs.Length);
        backChunk = Instantiate(chunkPrefabs[backIndex], transform);

        Vector3 backOffset = transform.position - backChunk.EndPosition;
        backChunk.transform.position += backOffset;
    }

    //Elige un trozo de escenario aleatorio y lo coloca justo donde termina el último trozo de la lista
    private void SpawnChunk()
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        BackgroundChunk newChunk = Instantiate(chunkPrefabs[randomIndex], transform);

        BackgroundChunk lastChunk = activeChunks[activeChunks.Count - 1];

        Vector3 currentSpawnPos = lastChunk.EndPosition;

        Vector3 offset = currentSpawnPos - newChunk.StartPosition;
        newChunk.transform.position += offset;

        activeChunks.Add(newChunk);
    }

    private void DespawnChunk()
    {
        BackgroundChunk oldChunk = activeChunks[0];
        activeChunks.RemoveAt(0);
        Destroy(oldChunk.gameObject);
    }
}