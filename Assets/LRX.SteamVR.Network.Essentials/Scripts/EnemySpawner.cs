﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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

    public void SpawnEnemies ()
    {
        // Do not spawn enemies if there are still enemies in the scene.
        if (NumEnemiesSpawned != 0)
            return;

        // Loop through spawn locations to spawn enemies
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // For Randomization of Spawn
            // var t = locations[Random.Range(0, locations.Count)];

            var enemy = (GameObject)Instantiate(enemyPrefab, locations[i].transform.position, locations[i].transform.rotation);
            NetworkServer.Spawn(enemy);
            NumEnemiesSpawned++;
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