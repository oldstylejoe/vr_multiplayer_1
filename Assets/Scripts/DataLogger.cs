using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour {
    private GameObject Player;
    private GameObject RightHand;
    private GameObject LeftHand;
    private List<GameObject> Enemies;
    private List<GameObject> Bullets;
    private bool isVR;

    void Start ()
    {
        Enemies = new List<GameObject>();
        Bullets = new List<GameObject>();

        isVR = false;
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
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject P in Players)
            {
                if (P.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    Player = P;
                    if (Player.GetComponent<VRPlayerController>())
                        isVR = true;
                }
            }
        }
        else
        {
            //Record Positions
            //Debug.Log("Player " + Player.transform.position + " " + Player.transform.rotation);
            Clock.markEvent("Player " + Player.transform.position + " " + Player.transform.rotation);
        }

        if (isVR)
        {
            if (!RightHand)
            {
                if(Player.transform.parent.transform.parent.transform.GetChild(1).childCount > 1)
                    RightHand = Player.transform.parent.transform.parent.transform.GetChild(1).GetChild(1).gameObject;
            }
            else
            {
                //Record Positions
                //Debug.Log("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
                Clock.markEvent("Right Hand " + RightHand.transform.position + " " + RightHand.transform.rotation);
            }

            if (!LeftHand)
            {
                if (Player.transform.parent.transform.parent.transform.GetChild(0).childCount > 1)
                    LeftHand = Player.transform.parent.transform.parent.transform.GetChild(0).GetChild(1).gameObject;
            }
            else
            {
                //Record Positions
                //Debug.Log("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
                Clock.markEvent("Left Hand " + LeftHand.transform.position + " " + LeftHand.transform.rotation);
            }
        }
    }

    private void RecordEnemies()
    {
        if (Enemies.Count != 0)
        {
            foreach(GameObject Enemy in Enemies)
            {
                //Record Positions
                if (Enemy)
                {
                    //Debug.Log("Enemy " + Enemy.transform.position + " " + Enemy.transform.rotation);
                    Clock.markEvent("Enemy " + Enemy.transform.position + " " + Enemy.transform.rotation);
                }
            }
        }
    }

    public void RecordBullets()
    {
        if (Bullets.Count != 0)
        {
            foreach(GameObject Bullet in Bullets)
            {
                //Record Positions
                if (Bullet)
                {
                    //Debug.Log("Bullet " + Bullet.transform.position + " " + Bullet.transform.rotation);
                    Clock.markEvent("Bullet " + Bullet.transform.position + " " + Bullet.transform.rotation);
                }
            }
        }
    }

    public void RecordButtonPress()
    {
        Clock.markEvent("Button Pressed");
    }

    public void RecordHit (GameObject hit)
    {
        string Victim = "";
        if (hit == Player)
            Victim = "Player, ";
        else if (hit.tag == "Enemy")
            Victim = "Enemy, ";
        
        if (Victim != "")
            Clock.markEvent(Victim + " " + "Hit");
    }

    #region Enemy List Access
    public void AddEnemy(GameObject newEnemy)
    {
        Enemies.Add(newEnemy);
    }

    public void RemoveEnemy(GameObject deadEnemy)
    {
        Enemies.Remove(deadEnemy);
    }
    #endregion

    #region Bullet List Access
    public void AddBullet(GameObject newBullet)
    {
        Bullets.Add(newBullet);
    }

    public void RemoveBullet(GameObject deadBullet)
    {
        Bullets.Remove(deadBullet);
    }
    #endregion
}
