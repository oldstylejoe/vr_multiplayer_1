using UnityEngine;
using UnityEngine.Networking;

public class RestartButton : NetworkBehaviour, ITouchable
{
    // This is the Restart Button code. 
    // Purpose: When the button is touched, the enemies will be respawned and
    //          the button and its column will moved to the end of the other hallway.

    // Get EnemySpawner 
    public EnemySpawner enemyspawner;

    private Vector3 Pos1;
    private Vector3 Pos2;

    void Start()
    {
        // Get the EnemySpawner GameObject
        if (!enemyspawner)
            enemyspawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        if (!enemyspawner)
            Debug.Log("Error: Need to include EnemySpawner gameobject in scene and add a reference to this script");

        // Column Positions
        Pos1 = transform.parent.position;
        Pos2 = Pos1 + new Vector3(-2.8f,0,0);
    }

    // This restarts the task.
    // The column moves to the other side and enemies respawned.
    private void ResetScene ()
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

    // This is for the controller part
    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetScene();
        }
    }

    // This is for the VR part
    public void Touch(NetworkInstanceId handId)
    {
        ResetScene();
    }
    public void Untouch(NetworkInstanceId handId)
    {
    }
}