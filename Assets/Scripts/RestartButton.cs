using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RestartButton : NetworkBehaviour, ITouchable
{
    // This is the Restart Button code. 
    // Purpose: When the button is touched, the enemies will be respawned and
    //          the button and its column will moved to the end of the other hallway.

    // Get EnemySpawner 
    public EnemySpawner enemyspawner;
    public GameObject WaitingWall;

    private Vector3 Pos1;
    private Vector3 Pos2;
    private Vector3 WallRestPos;
    private Vector3 WallPos;
    private Quaternion WallRestRot;
    private Quaternion WallRot;

    private DataLogger datalogger = null;

    void Start()
    {
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
            datalogger = dataloggerTest.GetComponent<DataLogger>();

        // Get the EnemySpawner GameObject
        if (!enemyspawner)
            enemyspawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        if (!enemyspawner)
            Debug.Log("Error: Need to include EnemySpawner gameobject in scene and add a reference to this script");

        // Waiting Wall Positions
        if (WaitingWall)
        {
            WallRestPos = WaitingWall.transform.position;
            WallRestRot = WaitingWall.transform.rotation;

            WallPos = new Vector3(0.6f,0,0);
            WallRot = Quaternion.Euler(0,0,-90);
        }
    }

    // This restarts the task.
    // The column moves to the other side and enemies respawned.
    private void ResetScene()
    {
        if (enemyspawner.EnemyCount != 0)
            return;

        if (datalogger)
            datalogger.RecordButtonPress();

        if (WaitingWall)
        {
            WaitingWall.transform.position = WallPos;
            WaitingWall.transform.rotation = WallRot;
        }

        StartCoroutine(WaitforReset(5f));
    }

    private IEnumerator WaitforReset(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Button has been used, Spawn enemies
        enemyspawner.SpawnEnemies();

        if (WaitingWall)
        {
            WaitingWall.transform.position = WallRestPos;
            WaitingWall.transform.rotation = WallRestRot;
        }
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