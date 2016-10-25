using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletCollide : NetworkBehaviour
{
    // Uses some code from the Bullet script from SteamVR Network Essentials

    // Lifetime of the bullet when fired
    public float lifeTime = 10f;

    private DataLogger datalogger;
    // Needed to identify the object in the server
    [SyncVar]
    public NetworkInstanceId projectileSourceId;

    void Start()
    {
        if (!isServer)
            return;
        // Start timer to destroy
        Invoke("DestroyMe", lifeTime);
        datalogger = GameObject.FindGameObjectWithTag("DataLogger").GetComponent<DataLogger>();
        datalogger.RecordShotFired(this.transform.position);
    }
    void DestroyMe()
    {
        NetworkServer.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Do not invoke collision methods below if the weapon is hit and do not perform locally
        if (!isServer)
            return;

        if (collision.gameObject.tag == "Weapon")
            return;

        // Destroy object on collision
        NetworkServer.Destroy(gameObject);
        if (collision.gameObject.tag == "Enemy")
        {
            GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>().EnemyCount--;
            datalogger.RemoveEnemy(collision.gameObject);
            Destroy(collision.gameObject);
            //add an explosion or something
            //destroy the projectile that just caused the trigger collision
            // Used to be handled with Event Manager
            // EventManager.TriggerEvent("Destroy");
        }
        else if (collision.gameObject.tag == "Player")
        {
            // Game Over Routine 
            // Need to be defined.
            Debug.Log("Game Over!!");
        }

        // Currently the target dies upon getting hit, so using the health code has been commented out
        // and is here as a placeholder in case this is used later

        //If target has health, take damage
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }

        datalogger.RecordHit(collision.gameObject);
    }

    /* Original Code
     * Moved up into OnCollisionEnter
    void OnTriggerEnter(Collider col)
    {
        //all projectile colliding game objects should be tagged "Enemy" or whatever in inspector but that tag must be reflected in the below if conditional
        //Debug.Log("gh3 " + col.gameObject.tag);
        if (col.gameObject.tag == "Enemy")
        {
            Destroy(col.gameObject);
            //add an explosion or something
            //destroy the projectile that just caused the trigger collision
            EventManager.TriggerEvent("Destroy");
        }

        if(col.gameObject.tag != "controller")
        {
            Destroy(gameObject, 1.0f);
        }
    }
    */
}
