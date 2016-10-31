using UnityEngine;
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
    private List<Transform> Enemies;
    private List<Transform> Bullets;

    void Start ()
    {
        Enemies = new List<Transform>();
        Bullets = new List<Transform>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        RecordPlayer();
        RecordEnemies();
        RecordBullets();
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
            Clock.markEvent("Player " + PlayerPos.x + " " + PlayerPos.y + " " + PlayerPos.z + " " + PlayerRot.x + " " + PlayerRot.y + " " + PlayerRot.z);
        }

        if (RightHand)
        {
            //Record Positions
            //Debug.Log("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
            Vector3 RightHandPos = RightHand.position;
            Quaternion RightHandRot = RightHand.rotation;
            Clock.markEvent("RightHand " + RightHandPos.x + " " + RightHandPos.y + " " + RightHandPos.z + " " + RightHandRot.x + " " + RightHandRot.y + " " + RightHandRot.z);
        }

        if (LeftHand)
        {
            //Record Positions
            //Debug.Log("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
            Vector3 LeftHandPos = LeftHand.position;
            Quaternion LeftHandRot = LeftHand.rotation;
            Clock.markEvent("LeftHand " + LeftHandPos.x + " " + LeftHandPos.y + " " + LeftHandPos.z + " " + LeftHandRot.x + " " + LeftHandRot.y + " " + LeftHandRot.z);
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
                    Clock.markEvent("Enemy " + EnemyPos.x + " " + EnemyPos.y + " " + EnemyPos.z + " " + EnemyRot.x + " " + EnemyRot.y + " " + EnemyRot.z);
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
                    Clock.markEvent("Bullet " + BulletPos.x + " " + BulletPos.y + " " + BulletPos.z + " " + BulletRot.x + " " + BulletRot.y + " " + BulletRot.z);
                }
            }
        }
    }

    public void RecordButtonPress()
    {
        Clock.markEvent("ButtonPressed");
    }

    public void RecordHit (GameObject hit)
    {
        string Victim = "";
        if (hit == Player)
            Victim = "Player, ";
        else if (hit.tag == "Enemy")
            Victim = "Enemy, ";
        
        if (Victim != "")
            Clock.markEvent(Victim + "Hit");
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
}