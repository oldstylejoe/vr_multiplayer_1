using UnityEngine;
using UnityEngine.Networking;

public class EnemyTurret : NetworkBehaviour, ITouchable
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
    public float fireRate = 1f;

    private GameObject target;
    private Vector3 lastKnownPosition = Vector3.zero;
    private Quaternion lookoutRotation;
    private float fireTimer = 0.0f;
    private DataLogger datalogger;

    void Start()
    {
        datalogger = GameObject.FindGameObjectWithTag("DataLogger").GetComponent<DataLogger>();
        datalogger.AddEnemy(this.gameObject);
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

            // Check when 2 seconds have passed and then fire.
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                if (Random.Range(0, 2) == 1)
                {
                    CmdFire();
                }
                fireTimer = 0.0f;
            }
        }
    }

    // 10% chance to see player when they enter field of view (trigger)
    void OnTriggerEnter (Collider other)
    {
        if (Random.Range(0, 10) != 1)
            return;

        if (!target)
        {
            if (other.gameObject.tag == "Player")
                target = other.gameObject;
        }
    }

    // see player when they are in the field of view (trigger)
    void OnTriggerStay (Collider other)
    {
        if (!target)
        {
            if (other.gameObject.tag == "Player")
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

    public void Touch(NetworkInstanceId handId)
    {
        Debug.Log("triggered");
    }

    public void Untouch(NetworkInstanceId handId)
    {
        Debug.Log("Relief");
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
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }
}