// Necessary Dependencies
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DataLogger : NetworkBehaviour {

    // Mohammad Alam
    // This is the DataLogger code
    // Purpose: Log the data to a file using Clocks file for format. 
    //  This works locally for each player.
    //  This also puts some information on the waiting wall.
    //
    // Data logged for VR and nonVR:
    //              Enemy Transform and Rotation
    //              Bullet Transform and Rotation
    //              Game Reset (Button Pressed)
    //              Object hit
    // Data logged for VR:
    //              Player body and hands Transform and Rotation
    // Data logged for nonVR: 
    //              Player object
    // All commented out Debug.Log calls are to test outputs

    // Player Information (RightHand and LeftHand only for VR)
    public Transform Player;
    public Transform RightHand;
    public Transform LeftHand;
    // Boolean for turning on and off file I/O
    public bool WriteToFile = true;
    // Unity Text UI for the waiting wall
    public Text WaitingWallText;

    // List of Enemies and Bullets in Scene for logging
    private List<Transform> Enemies = new List<Transform>();
    private List<Transform> Bullets = new List<Transform>();
    // Counters for Trial Number, number of wins, and number of losses
    private int trialCount = 0;
    private int winCount = 0;
    private int lossCount = 0;
    // Boolean for when the trial is lost. This happens when either player is shot.
    // The SyncVar attribute allows for either player to be shot and have it be recorded for both.
    [SyncVar]
    private bool trialLost = false;

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
            LogHandler.markEvent("Player " + PlayerPos.x + " " + PlayerPos.y + " " + PlayerPos.z + " " + PlayerRot.x + " " + PlayerRot.y + " " + PlayerRot.z);
        }

        if (RightHand)
        {
            //Record Player Right Hand Positions and Rotations
            //Debug.Log("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
            Vector3 RightHandPos = RightHand.position;
            Quaternion RightHandRot = RightHand.rotation;
            LogHandler.markEvent("RightHand " + RightHandPos.x + " " + RightHandPos.y + " " + RightHandPos.z + " " + RightHandRot.x + " " + RightHandRot.y + " " + RightHandRot.z);
        }

        if (LeftHand)
        {
            //Record Player Left Hand Positions and Rotations
            //Debug.Log("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
            Vector3 LeftHandPos = LeftHand.position;
            Quaternion LeftHandRot = LeftHand.rotation;
            LogHandler.markEvent("LeftHand " + LeftHandPos.x + " " + LeftHandPos.y + " " + LeftHandPos.z + " " + LeftHandRot.x + " " + LeftHandRot.y + " " + LeftHandRot.z);
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
                    LogHandler.markEvent("Enemy " + EnemyPos.x + " " + EnemyPos.y + " " + EnemyPos.z + " " + EnemyRot.x + " " + EnemyRot.y + " " + EnemyRot.z);
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
                    LogHandler.markEvent("Bullet " + BulletPos.x + " " + BulletPos.y + " " + BulletPos.z + " " + BulletRot.x + " " + BulletRot.y + " " + BulletRot.z);
                }
            }
        }
    }

    // Every Button Press is recorded and each section of data is segmented accordingly.
    public void RecordButtonPress()
    {
        if (WriteToFile)
            LogHandler.markEvent(System.Environment.NewLine + "Trial " + trialCount + " ");

        if (WaitingWallText)
        {
            RpcChangeWallText();
        }
    }

    // Record Player and Enemy hit information
    // Also sets trialLost to be true when Player is hit to denote a loss
    public void RecordHit (GameObject hit)
    {
        string Victim = "";
        if (hit.tag == "Player")
        {
            Victim = "Player, ";
            trialLost = true;
        }
        else if (hit.tag == "Enemy")
            Victim = "Enemy, ";

        if (Victim != "")
        {
            if (WriteToFile)
                LogHandler.markEvent(Victim + "Hit");
        }
    }

    // List access from other functions, such as BulletCollide and EnemyTurret
    #region Enemy List Access
    public void AddEnemy(Transform newEnemy)
    {
        Enemies.Add(newEnemy);
    }

    public void RemoveEnemy(Transform deadEnemy)
    {
        Enemies.Remove(deadEnemy);
    }
    #endregion

    #region Bullet List Access
    public void AddBullet(Transform newBullet)
    {
        Bullets.Add(newBullet);
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
        if (trialLost)
        {
            lossCount++;
            trialLost = false;
        }
        else if (trialCount != 0)
            winCount++;

        trialCount++;

        WaitingWallText.text = "Please Wait for Trial " + trialCount + " to start" + System.Environment.NewLine + System.Environment.NewLine +
                               "Current Stats:" + System.Environment.NewLine +
                               "Wins:    " + winCount + System.Environment.NewLine +
                               "Losses:  " + lossCount + System.Environment.NewLine;
    }
}