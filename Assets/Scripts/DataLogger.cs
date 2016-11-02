using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DataLogger : NetworkBehaviour {

    // This is the DataLogger code
    // Purpose: Log the data to a file using Clocks file for format
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

    public Transform Player;
    public Transform RightHand;
    public Transform LeftHand;
    public bool WriteToFile = true;
    public Text WaitingWallText;

    private List<Transform> Enemies;
    private List<Transform> Bullets;
    [SyncVar]
    private int trialCount = 0;
    [SyncVar]
    private int winCount = 0;
    [SyncVar]
    private int lossCount = 0;
    private bool trialLost = false;
    private static int times = 0;

    void Start ()
    {
        Enemies = new List<Transform>();
        Bullets = new List<Transform>();
    }

	void FixedUpdate () {
        if (WriteToFile)
        {
            RecordPlayer();
            RecordEnemies();
            RecordBullets();
        }
	}

    private void RecordPlayer ()
    {
        if (!Player)
        {
            return;
        }
        else
        {
            //Record Positions
            //Debug.Log("Player " + Player.transform.position + " " + Player.transform.rotation);
            Vector3 PlayerPos = Player.position;
            Quaternion PlayerRot = Player.rotation;
            LogHandler.markEvent("Player " + PlayerPos.x + " " + PlayerPos.y + " " + PlayerPos.z + " " + PlayerRot.x + " " + PlayerRot.y + " " + PlayerRot.z);
        }

        if (RightHand)
        {
            //Record Positions
            //Debug.Log("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
            Vector3 RightHandPos = RightHand.position;
            Quaternion RightHandRot = RightHand.rotation;
            LogHandler.markEvent("RightHand " + RightHandPos.x + " " + RightHandPos.y + " " + RightHandPos.z + " " + RightHandRot.x + " " + RightHandRot.y + " " + RightHandRot.z);
        }

        if (LeftHand)
        {
            //Record Positions
            //Debug.Log("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
            Vector3 LeftHandPos = LeftHand.position;
            Quaternion LeftHandRot = LeftHand.rotation;
            LogHandler.markEvent("LeftHand " + LeftHandPos.x + " " + LeftHandPos.y + " " + LeftHandPos.z + " " + LeftHandRot.x + " " + LeftHandRot.y + " " + LeftHandRot.z);
        }
    }

    private void RecordEnemies()
    {
        if (Enemies.Count != 0)
        {
            foreach(Transform Enemy in Enemies)
            {
                //Record Positions
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

    public void RecordBullets()
    {
        if (Bullets.Count != 0)
        {
            foreach(Transform Bullet in Bullets)
            {
                //Record Positions
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

    public void RecordButtonPress()
    {
        if (trialLost)
        {
            lossCount++;
            trialLost = false;
        }
        else if (trialCount != 0)
            winCount++;

        trialCount++;

        if (WriteToFile)
            LogHandler.markEvent(System.Environment.NewLine + "Trial " + trialCount);

        if (WaitingWallText)
        {
            RpcChangeWallText();
        }
    }

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

    [ClientRpc]
    private void RpcChangeWallText()
    {
        times++;

        WaitingWallText.text = "Please Wait for Trial " + trialCount + " to start" + System.Environment.NewLine + System.Environment.NewLine +
                               "Current Stats:" + System.Environment.NewLine +
                               "Wins:    " + winCount + System.Environment.NewLine +
                               "Losses:  " + lossCount + System.Environment.NewLine;

        Debug.Log("Called: " + times + " times");
    }
}