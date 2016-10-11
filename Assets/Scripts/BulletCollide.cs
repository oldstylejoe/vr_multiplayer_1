using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BulletCollide : NetworkBehaviour
{
    // Uses some code from the Bullet script from SteamVR Network Essentials

    public float lifeTime = 10f;
    [SyncVar]
    public NetworkInstanceId projectileSourceId;

    void Start()
    {
        if (!isServer)
            return;
        Invoke("DestroyMe", lifeTime);
    }
    void DestroyMe()
    {
        NetworkServer.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
            return;

        if (collision.gameObject.tag == "Weapon")
            return;

        NetworkServer.Destroy(gameObject);
        GameObject hit = collision.gameObject;
        if (hit.gameObject.tag == "Enemy")
        {
            GameObject.FindWithTag("EnemySpawner").GetComponent<EnemySpawner>().EnemyCount--;
            Destroy(hit.gameObject);
            //add an explosion or something
            //destroy the projectile that just caused the trigger collision
            //EventManager.TriggerEvent("Destroy");
        }

        // Currently the target dies upon getting hit, so using the health code has been commented out
        // and is here as a placeholder in case this is used later
        /*
        //If target has health, take damage
        var health = hit.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(10);
        }
        */
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
