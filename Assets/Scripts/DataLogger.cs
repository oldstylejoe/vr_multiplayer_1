using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour {
    private GameObject Player;
    private List<GameObject> Enemies;

    void Start ()
    {
        Enemies = new List<GameObject>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        RecordPlayer();
        RecordEnemies();
	}

    private void RecordPlayer ()
    {
        if (!Player)
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject P in Players)
            {
                if (P.GetComponent<PlayerController>().isLocalPlayer)
                {
                    Player = P;
                }
            }
        }
        else
        {
            //Record Positions
            Debug.Log("Player, " + Time.time + ", " + Player.transform.position + ", " + Player.transform.rotation);
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
                    Debug.Log("Enemy, " + Time.time + ", " + Enemy.transform.position + ", " + Enemy.transform.rotation);
                }
                else
                {
                    Enemies.Remove(Enemy);
                }
            }
        }
    }

    public void RecordButtonPress()
    {
        Debug.Log("Button Pressed, " + Time.time);
    }

    public void RecordShotFired (Vector3 BulletSpawn)
    {
        Debug.Log("Gun Fired, " + Time.time + ", " + BulletSpawn);
    }

    public void RecordHit (GameObject hit)
    {
        string Victim = "";
        if (hit == Player)
            Victim = "Player, ";
        else if (hit.tag == "Enemy")
            Victim = "Enemy, ";
        
        if (Victim != "")
            Debug.Log(Victim + Time.time + ", " + "Hit");
    }

    public void AddEnemy(GameObject newEnemy)
    {
        Enemies.Add(newEnemy);
    }

    public void RemoveEnemy(GameObject deadEnemy)
    {
        Enemies.Remove(deadEnemy);
    }
}
