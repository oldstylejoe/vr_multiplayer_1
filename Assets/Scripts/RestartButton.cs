using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RestartButton : NetworkBehaviour, ITouchable
{
    // This is the Restart Button code. 
    // Purpose: When the button is touched, a wall will appear 
    //          to tell the players not to move forward for RespawnTimer
    //          seconds. After those seconds, it

    // Respawn Timer Constant
    public float RespawnTimer = 5f;
    // Get EnemySpawner 
    public EnemySpawner enemyspawner;
    // Get our Waiting Wall Object
    public GameObject WaitingWall;

    // DataLogger Script from Datalogger object
    private DataLogger datalogger = null;
    // Check to see coroutine for respawning is running
    private bool Respawning = false;
    private Vector3 WaitPos;
    private Vector3 GoPos;

    void Start()
    {
        // Datalogger Initialize
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
            datalogger = dataloggerTest.GetComponent<DataLogger>();

        // Get the EnemySpawner GameObject
        if (!enemyspawner)
            enemyspawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        if (!enemyspawner)
            Debug.LogError("Error: Need to include EnemySpawner gameobject in scene and add a reference to this script", enemyspawner);

        // Check if a Waiting Wall is used and then lower it to hide it
        if (WaitingWall)
        {
            WaitPos = WaitingWall.transform.position;
            Debug.Log(WaitPos);
            GoPos = WaitPos + new Vector3(0, -4, 0);
            WaitingWall.transform.position = GoPos;
        }
    }

    // This restarts the task.
    // A wall appears for RespawnTimer seconds and then the Enemies Spawn
    private void ResetScene()
    {
        // Check if all enemies have been taken out
        if (enemyspawner.EnemyCount != 0)
            return;

        // Record press
        if (datalogger)
            datalogger.RecordButtonPress();

        // Raise WaitingWall for RespawnTimer seconds
        if (WaitingWall)
        {
            WaitingWall.transform.position = WaitPos;
        }

        StartCoroutine(WaitforReset(RespawnTimer));
    }

    private IEnumerator WaitforReset(float waitTime)
    {
        if (Respawning)
            yield break;

        Respawning = true;

        yield return new WaitForSeconds(waitTime);

        // Button has been used, Spawn enemies
        enemyspawner.SpawnEnemies();

        if (WaitingWall)
        {
            WaitingWall.transform.position = GoPos;
        }

        Respawning = false;
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