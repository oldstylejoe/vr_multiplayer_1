/* Code by Mohammad Alam
 * Purpose: Log the data to a file using Clocks file for format. 
 *   This works locally for each player.
 *   This also puts some information on the waiting wall.
 * 
 * Data Logged for all cases:
 *      - Enemy Transform and Rotation
 *      - Bullet Transform and Rotation
 *      - Game Reset (Button Pressed)
 *      - Object hit
 *      
 * Data Logged for VR:
 *      - Player body and hands Transform and Rotation
 * Data Logged for VR:
 *      - Player object
 *      
 * All commented out Debug.Log calls are to test outputs
 */

// Necessary Dependencies
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DataLogger : NetworkBehaviour {

    // Player Information (RightHand and LeftHand only for VR)
    public Transform Player;
    public Transform RightHand;
    public Transform LeftHand;
    // Unity Text UI for the waiting wall
    public Text WaitingWallText;

    private string subjName;
    // Boolean for turning on and off file I/O
    private const bool WriteToFile = true;
    // Boolean for adding a header
    private const bool giveHeader = true;
    // List of Enemies and Bullets in Scene for logging
    private List<Transform> Enemies = new List<Transform>();
    private static int enemyCounter = 0;
    private List<Transform> Bullets = new List<Transform>();
    private static int bulletCounter = 0;
    // Counters for Trial Number, number of wins, and number of losses
    private int trialCount = 0;
    private int winCount = 0;
    private int lossCount = 0;
    // Boolean for when the trial is lost. This happens when either player is shot.
    // The SyncVar attribute allows for either player to be shot and have it be recorded for both.
    [SyncVar]
    private bool trialLost = false;

    void Start ()
    {
        // Create Header
        if (WriteToFile && giveHeader)
            LogHandler.markEvent("Object_Or_Event Trial_Count Pos_X Pos_Y Pos_Z Rot_X Rot_Y Rot_Z");
    }

	void FixedUpdate () {
        // Write to file every FixedUpdate
        if (WriteToFile)
        {
            RecordPlayer();
            RecordEnemies();
            RecordBullets();
        }
    }

    // For recording player information. The public Player GameObject Variables are filled in using
    // external scripts: PlayerController and VrPlayerController
    private void RecordPlayer ()
    {
        if (!Player)
        {
            return;
        }
        else
        {
            //Record Player Body Positions and Rotations
            //Debug.Log("Player " + Player.transform.position + " " + Player.transform.rotation);
            Vector3 PlayerPos = Player.position;
            Quaternion PlayerRot = Player.rotation;
            LogHandler.markEvent("LocalPlayer " + trialCount + " " + PlayerPos.x + " " + PlayerPos.y + " " + PlayerPos.z + " " + PlayerRot.x + " " + PlayerRot.y + " " + PlayerRot.z);
        }

        if (RightHand)
        {
            //Record Player Right Hand Positions and Rotations
            //Debug.Log("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
            Vector3 RightHandPos = RightHand.position;
            Quaternion RightHandRot = RightHand.rotation;
            LogHandler.markEvent("LocalPlayer_RightHand " + trialCount + " " + RightHandPos.x + " " + RightHandPos.y + " " + RightHandPos.z + " " + RightHandRot.x + " " + RightHandRot.y + " " + RightHandRot.z);
        }

        if (LeftHand)
        {
            //Record Player Left Hand Positions and Rotations
            //Debug.Log("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
            Vector3 LeftHandPos = LeftHand.position;
            Quaternion LeftHandRot = LeftHand.rotation;
            LogHandler.markEvent("LocalPlayer_LeftHand " + trialCount + " " + LeftHandPos.x + " " + LeftHandPos.y + " " + LeftHandPos.z + " " + LeftHandRot.x + " " + LeftHandRot.y + " " + LeftHandRot.z);
        }
    }

    // Record Positions and Rotations of all the enemies in the Enemies List
    private void RecordEnemies()
    {
        if (Enemies.Count != 0)
        {
            foreach(Transform Enemy in Enemies)
            {
                if (Enemy)
                {
                    //Debug.Log("Enemy " + Enemy.transform.position + " " + Enemy.transform.rotation);
                    Vector3 EnemyPos = Enemy.position;
                    Quaternion EnemyRot = Enemy.rotation;
                    LogHandler.markEvent(Enemy.name + " " + trialCount + " " + EnemyPos.x + " " + EnemyPos.y + " " + EnemyPos.z + " " + EnemyRot.x + " " + EnemyRot.y + " " + EnemyRot.z);
                }
            }
        }
    }

    // Record Positions and Rotations of all the bullets in the Bullets List
    public void RecordBullets()
    {
        if (Bullets.Count != 0)
        {
            foreach(Transform Bullet in Bullets)
            {
                if (Bullet)
                {
                    //Debug.Log("Bullet " + Bullet.transform.position + " " + Bullet.transform.rotation);
                    Vector3 BulletPos = Bullet.position;
                    Quaternion BulletRot = Bullet.rotation;
                    LogHandler.markEvent(Bullet.name + " " + trialCount + " " + BulletPos.x + " " + BulletPos.y + " " + BulletPos.z + " " + BulletRot.x + " " + BulletRot.y + " " + BulletRot.z);
                }
            }
        }
    }

    // Every Button Press is recorded and each section of data is segmented accordingly.
    public void RecordButtonPress()
    {
        if (WaitingWallText)
        {
            RpcChangeWallText();
        }
    }

    // Record Player and Enemy hit information
    // Also sets trialLost to be true when Player is hit to denote a loss
    public void RecordHit (Transform hit)
    {
        Vector3 hitPos = hit.position;
        Quaternion hitRot = hit.rotation;
        if (WriteToFile)
        {
            if (hit.gameObject == Player.gameObject)
                LogHandler.markEvent("LocalPlayer" + "Hit " + trialCount + " " + hitPos.x + " " + hitPos.y + " " + hitPos.z + " " + hitRot.x + " " + hitRot.y + " " + hitRot.z);
            else
                LogHandler.markEvent(hit.name + "Hit " + trialCount + " " + hitPos.x + " " + hitPos.y + " " + hitPos.z + " " + hitRot.x + " " + hitRot.y + " " + hitRot.z);
        }

        if(hit.CompareTag("Player"))
            trialLost = true;
    }

    // List access from other functions, such as BulletCollide and EnemyTurret
    #region Enemy List Access
    public void AddEnemy(Transform newEnemy)
    {
        newEnemy.name = "Enemy" + enemyCounter;
        Enemies.Add(newEnemy);
        enemyCounter++;
    }

    public void RemoveEnemy(Transform deadEnemy)
    {
        Enemies.Remove(deadEnemy);
    }
    #endregion

    #region Bullet List Access
    public void AddBullet(Transform newBullet)
    {
        newBullet.name = "Bullet" + bulletCounter;
        Bullets.Add(newBullet);
        bulletCounter++;
    }

    public void RemoveBullet(Transform deadBullet)
    {
        Bullets.Remove(deadBullet);
    }
    #endregion

    // Change the WaitignWallText
    // This RpcCommand occurs on the server and affects all clients, so that everyone
    // has the correct information.
    [ClientRpc]
    private void RpcChangeWallText()
    {
        if (trialCount != 0)
        {
            if (trialLost)
            {
                lossCount++;
                trialLost = false;
            }
            else
                winCount++;
        }

        trialCount++;

        WaitingWallText.text = "Please Wait for Trial " + trialCount + " to start" + System.Environment.NewLine + System.Environment.NewLine +
                               "Current Stats:" + System.Environment.NewLine +
                               "Wins:    " + winCount + System.Environment.NewLine +
                               "Losses:  " + lossCount + System.Environment.NewLine;
    }
}