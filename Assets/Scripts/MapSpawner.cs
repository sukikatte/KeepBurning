using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour {
    public Transform player;
    public MapChunk[] chunkPrefabs;
    
    private Queue<MapChunk> chunks = new Queue<MapChunk>();
    private MapChunk lastChunk;

    void Start() {
        // spawn initial map chunks
        for (int i = 0; i < 4; i++) {
            SpawnChunk();
        }
    }

    void Update() {
        if (player == null || lastChunk == null)
            return;
        
        // if the distance between player and last chunk is close, spawn new chunk
        if (lastChunk.endPoint.position.z - player.position.z < 40f) {
            SpawnChunk();
        }

        // remove old chunk if too many exist
        if (chunks.Count > 6) {
            MapChunk oldChunk = chunks.Dequeue();
            Destroy(oldChunk.gameObject);
        }
    }

    void SpawnChunk() {
        // select a random chunk prefab
        MapChunk prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        
        MapChunk newChunk = Instantiate(prefab);

        // first chunk at origin
        if (lastChunk == null) {
            newChunk.transform.position = Vector3.zero;
        }
        else {
            // connecting the latest chunk's endpoint 
            Vector3 offset = newChunk.startPoint.position - newChunk.transform.position;
            newChunk.transform.position = lastChunk.endPoint.position - offset;
        }

        // save chunk in queue
        chunks.Enqueue(newChunk);
        lastChunk = newChunk;
    }
}