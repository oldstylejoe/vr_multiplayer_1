/* Original Code from SteamVR Essentials
 * Purpose: Handle Enemy spawning
 * 
 * Modifications made by Mohammad Alam
 *  - Changed spawning method 
 *      - Now handled with event manager (Enemy count incrementing/decrementing and spawning)
 *      - Spawnpoints now determine number of enemies spawned
 *      - 1 Enemy is always spawned. Subsequent enemies have a 50% probability of spawning.
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : NetworkBehaviour {
    // Enemy Spawning Code - A network replacement of events for the network implementation
    // Purpose: Spawn enemies on start at transform locations set in the inspector

    public GameObject enemyPrefab;
    public List<GameObject> locations;

    private int numberOfEnemies;

    [SyncVar]
    private int NumEnemiesSpawned = 0;

    void Start ()
    {
        // Maximum number of enemies determined by number of spawnpoints
        numberOfEnemies = locations.Count;
    }

    // EventManager Calls
    void OnEnable()
    {
        EventManager.StartListening("Spawn", SpawnEnemies);
        EventManager.StartListening("EnemyInc", EnemyInc);
        EventManager.StartListening("EnemyDec", EnemyDec);
    }

    void OnDisable()
    {
        EventManager.StopListening("Spawn", SpawnEnemies);
        EventManager.StopListening("EnemyInc", EnemyInc);
        EventManager.StopListening("EnemyDec", EnemyDec);
    }

    public void SpawnEnemies ()
    {
        if (!isServer)
            return;

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

            indexes.Remove(loc);

            if (Random.Range(0, 2) == 0)
                break;
        }
    }

    // Events for the NumEnemiesSpawned allows for NumEnemiesSpawned variable to be changed out of scope.
    public void EnemyInc ()
    {
        NumEnemiesSpawned++;
    }

    public void EnemyDec ()
    {
        NumEnemiesSpawned--;
    }

    public int EnemyCount
    {
        get { return NumEnemiesSpawned; }
    }
}