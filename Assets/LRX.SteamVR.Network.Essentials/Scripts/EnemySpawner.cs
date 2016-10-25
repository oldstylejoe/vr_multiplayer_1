using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : NetworkBehaviour {
    // Enemy Spawning Code - A network replacement of events for the network implementation
    // Purpose: Spawn enemies on start at transform locations set in the inspector

    public GameObject enemyPrefab;
    public int numberOfEnemies;

    public List<GameObject> locations;

    [SyncVar]
    private int NumEnemiesSpawned = 0;

    public override void OnStartServer()
    {
        // Server has started, spawn enemies.
        SpawnEnemies();
    }

    void OnEnable()
    {
        EventManager.StartListening("Destroy", SpawnEnemies);
    }

    void OnDisable()
    {
        EventManager.StopListening("Destroy", SpawnEnemies);
    }

    public void SpawnEnemies ()
    {    
        // Do not spawn enemies if there are still enemies in the scene.
        if (NumEnemiesSpawned != 0)
            return;

        List<int> indexes = Enumerable.Range(0,locations.Count).ToList();

        // Loop through spawn locations to spawn enemies
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // For Randomization of Spawn
            var loc = indexes[Random.Range(0, indexes.Count)];

            var enemy = (GameObject)Instantiate(enemyPrefab, locations[loc].transform.position, locations[loc].transform.rotation);
            NetworkServer.Spawn(enemy);
            NumEnemiesSpawned++;

            indexes.Remove(loc);

            if (Random.Range(0, 2) == 0)
                break;
        }
    }

    // Getter and Setter EnemyCount allows for NumEnemiesSpawned variable to be changed out of scope.
    public int EnemyCount
    {
        get
        {
            return NumEnemiesSpawned;
        }

        set
        {
            if (!isServer)
                return;

            NumEnemiesSpawned = value;
        }
    }
}