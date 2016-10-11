using UnityEngine;
using UnityEngine.Networking;

public class RestartButton : NetworkBehaviour, ITouchable
{
    // This is the Restart Button code. 
    // Purpose: When the button is touched, the enemies will be respawned and
    //          the button and its column will moved to the end of the other hallway.

    private Vector3 Pos1;
    private Vector3 Pos2;

    private EnemySpawner enemyspawner;

    void Start()
    {
        // Get the EnemySpawner GameObject
        enemyspawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        // Column Positions
        Pos1 = transform.parent.position;
        Pos2 = Pos1 + new Vector3(-2.8f,0,0);
    }

    public void Touch(NetworkInstanceId handId)
    {
        if (enemyspawner.EnemyCount != 0)
            return;
        // Button has been used, Spawn enemies
        enemyspawner.SpawnEnemies();
        if (transform.parent.localPosition == Pos1)
            transform.parent.localPosition = Pos2;
        else
            transform.parent.localPosition = Pos1;
    }
    public void Untouch(NetworkInstanceId handId)
    {
    }
}