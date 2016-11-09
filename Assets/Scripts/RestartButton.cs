/* Code by Mohammad Alam
 * Purpose: Code to restart game or start new trial
 *      When the button is touched, a wall will appear to 
 *      tell the players not to move forward for RespawnTimer 
 *      seconds. After those seconds, it will disappear and
 *      the enemies will respawn.
 * 
 * Interacts with the DataLogger to log event of new trial start.
 * and to update WaitingWallText.
 * 
 * Plays a little beeping sound when touched.
 * 
 */

// Beeping Sound Courtesy of Freesound.org: http://freesound.org/people/MeTwo99/sounds/148694/?page=1#comment

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RestartButton : NetworkBehaviour, ITouchable
{
    // Respawn Timer Constant
    public float RespawnTimer = 10f;
    // Get EnemySpawner 
    public EnemySpawner enemyspawner;
    // Get our Waiting Wall Object
    public GameObject WaitingWall;
    // Get our Material for color
    public Material buttonMat;
    public Color NonUseColor = new Color(1f, 0f, 0f);
    public Color UseColor = new Color(0f, 1f, 0f);

    // DataLogger Script from Datalogger object
    private DataLogger datalogger = null;
    // Check to see coroutine for respawning is running
    private bool Respawning = false;
    private Vector3 WaitPos;
    private Vector3 GoPos;
    // Beepy Sound
    private AudioSource audioSrc;

    void Start()
    {
        // Datalogger Initialize
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
            datalogger = dataloggerTest.GetComponent<DataLogger>();

        audioSrc = GetComponent<AudioSource>();

        // Get the EnemySpawner GameObject
        if (!enemyspawner)
            enemyspawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>();

        if (!enemyspawner)
            Debug.LogError("Error: Need to include EnemySpawner gameobject in scene and add a reference to this script", enemyspawner);

        // Check if a Waiting Wall is used and then lower it to hide it
        if (WaitingWall)
        {
            WaitPos = WaitingWall.transform.position;
            GoPos = WaitPos + new Vector3(0, -4, 0);
            WaitingWall.transform.position = GoPos;
        }

        buttonMat.color = NonUseColor;
    }

    // This restarts the task.
    // A wall appears for RespawnTimer seconds and then the Enemies Spawn
    private void ResetScene()
    {
        // Check if all enemies have been taken out
        if (enemyspawner.EnemyCount != 0  || Respawning)
            return;

        // Record press
        if (datalogger)
            datalogger.RecordButtonPress();

        StartCoroutine(WaitforReset(RespawnTimer));
    }

    [ClientRpc]
    private void RpcWaiting(bool waiting)
    {
        if (waiting)
        {
            // Raise WaitingWall for RespawnTimer seconds
            if (WaitingWall)
            {
                WaitingWall.transform.position = WaitPos;
            }
            buttonMat.color = UseColor;
        }
        else
        {
            if (WaitingWall)
            {
                WaitingWall.transform.position = GoPos;
            }
            buttonMat.color = NonUseColor;
        }
    }

    [Command]
    private void CmdReadyBeep (float pitchSet)
    {
        if (audioSrc)
        {
            RpcPlayBeep(pitchSet);
        }
    }

    [ClientRpc]
    private void RpcPlayBeep (float pitchSet)
    {
        audioSrc.pitch = pitchSet;
        audioSrc.Play();

    }

    private IEnumerator WaitforReset(float totalWaitTime)
    {
        if (Respawning)
            yield break;

        float startTime = Time.time;

        Respawning = true;
        RpcWaiting(true);

        CmdReadyBeep(0.5f);

        yield return new WaitForSeconds(totalWaitTime - 3f);

        float waitSec = Random.Range(1,1.5f);

        while (totalWaitTime > Time.time - startTime)
        {
            CmdReadyBeep(1f);
            yield return new WaitForSeconds(waitSec);
        }

        // Button has been used, Spawn enemies
        enemyspawner.SpawnEnemies();

        RpcWaiting(false);
        Respawning = false;

        if (audioSrc)
        {
            audioSrc.pitch = 2f;
            audioSrc.Play();
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