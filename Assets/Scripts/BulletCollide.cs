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

        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
            datalogger = dataloggerTest.GetComponent<DataLogger>();
        if(datalogger)
            datalogger.AddBullet(this.transform);
    }
    void DestroyMe()
    {
        NetworkServer.Destroy(gameObject);
        if (datalogger)
            datalogger.RemoveBullet(this.transform);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Do not invoke collision methods below if the weapon is hit and do not perform locally
        if (!isServer)
            return;

        if (collision.gameObject.CompareTag("Weapon"))
            return;

        // Destroy object on collision
        NetworkServer.Destroy(gameObject);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EventManager.TriggerEvent("EnemyDec");
            if (datalogger)
                datalogger.RemoveEnemy(collision.transform);
            Destroy(collision.gameObject);
            //add an explosion or something
            //destroy the projectile that just caused the trigger collision
            // Used to be handled with Event Manager
            // EventManager.TriggerEvent("Destroy");
        }
        /*
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Game Over Routine???
        }
        */

        //If target has health, take damage
        var health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }

        if (datalogger)
        {
            datalogger.RecordHit(collision.transform);
            datalogger.RemoveBullet(this.transform);
        }
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
