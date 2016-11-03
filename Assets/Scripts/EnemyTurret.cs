using UnityEngine;
using UnityEngine.Networking;

public class EnemyTurret : NetworkBehaviour
{
    // Enemy AI code to make the Enemy act like a turret.
    /*
     * 
     * Currently, the enemy is coded to turn its whole body accordingly when it sees
     * the target (Player) and has a chance to fire every 2 seconds.
     * The gun will randomly change rotation when firing.
     * 
     */
    public float rotationSpeed = 20f;
    // Necessary variables for shooting
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Transform gun;
    public float bulletSpeed = 6f;
    public float startFireRate = 0.100f;
    public float fireRate = 1f;

    private GameObject target;
    private Vector3 lastKnownPosition = Vector3.zero;
    private Quaternion lookoutRotation;
    private Quaternion RestRotation;
    private float fireTimer = 0.0f;
    private DataLogger datalogger;
    private bool fireFaster = false;

    // Necessary to add to enemy count when spawned
    public EnemySpawner enemyspawner;

    void Start()
    {
        RestRotation = transform.rotation;

        EventManager.TriggerEvent("EnemyInc");

        // Check for DataLogger Object
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
        {
            datalogger = dataloggerTest.GetComponent<DataLogger>();
            datalogger.AddEnemy(this.transform);
        }
    }

    void Update()
    {
        // Perform target actions when targets have been found
        if (target)
        {
            // If the target has moved, then reorient
            if (lastKnownPosition != target.transform.position)
            {
                lastKnownPosition = target.transform.position;

                // Get needed rotation to look at forward pointing vector
                lookoutRotation = Quaternion.LookRotation(lastKnownPosition - transform.position);
            }

            if (transform.rotation != lookoutRotation)
            {
                // Rotate from current rotation to lookout rotation at turning speed
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookoutRotation, rotationSpeed * Time.deltaTime);
            }

            // Check when fireRate seconds have passed and then fire.
            fireTimer += Time.deltaTime;
            if (!fireFaster)
            {
                if (fireTimer >= startFireRate)
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        CmdFire();
                    }
                    fireTimer = 0.0f;
                    fireFaster = true;
                }
            }
            else if (fireTimer >= fireRate)
            {
                if (Random.Range(0, 2) == 1)
                {
                    CmdFire();
                }
                fireTimer = 0.0f;
            }
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, RestRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // 10% chance to see player when they enter field of view (trigger)
    void OnTriggerEnter (Collider other)
    {
        if (Random.Range(0, 10) != 1)
            return;

        if (!target)
        {
            if (other.gameObject.CompareTag("Player"))
                target = other.gameObject;
        }
    }

    // see player when they are in the field of view (trigger)
    void OnTriggerStay (Collider other)
    {
        if (!target)
        {
            if (other.gameObject.CompareTag("Player"))
                target = other.gameObject;
        }
    }

    // Lose target when out of line of sight.
    void OnTriggerExit(Collider other)
    {
        if (target)
        {
            if (other.gameObject == target)
                target = null;
        }
    }

    // This is borrowed from the original gun script.
    // This [Command] code for shooting is called on the Client …
    // … but it is run on the Server!
    [Command]
    protected void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation * Quaternion.Euler(Random.Range(-10.0f,10.0f), Random.Range(-10.0f, 10.0f), 0f));

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}