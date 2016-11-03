using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    // Slight modifications made to Steam VR Network Essentials code for aim clamping and correct movement

	public GameObject bulletPrefab;
	private MouseLook mouseLook;
	public Transform bulletSpawn;
    public float speed;

    private Vector3 PosOffset;
    
    private DataLogger datalogger;
    public override void OnStartLocalPlayer()
	{
        transform.position = new Vector3(transform.position.x + -1f, transform.position.y + .8f, transform.position.z);
        transform.rotation = Quaternion.Euler(0,90,0);
		GetComponent<Renderer>().material.color = Color.blue;

        // attach camera to player.. 3rd person view..
        Camera.main.transform.parent = transform;
		Camera.main.transform.localPosition = new Vector3 (0, .5f, 0);
		Camera.main.transform.localRotation = Quaternion.Euler (6.31f, 0, 0);

        PosOffset = Camera.main.transform.position - transform.GetChild(2).position;

        // Note: Modification was made to take out the HP label and add in a new damage image canvas.
        //       This means the object has a different array of children. The original prefab will no
        //       longer work with this code. Only the new modified one will.
        transform.GetChild(3).GetComponent<Canvas>().worldCamera = Camera.main;
        transform.GetChild(3).GetComponent<Canvas>().planeDistance = .4f;
        GetComponent<Health>().damageImage = transform.GetChild(3).GetChild(0).GetComponent<Image>();

        mouseLook = new MouseLook ();
		mouseLook.Init (transform, Camera.main.transform);

        // Check for DataLogger Object
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
        {
            datalogger = dataloggerTest.GetComponent<DataLogger>();
            datalogger.Player = gameObject.transform;
            datalogger.LeftHand = null;
            datalogger.RightHand = null;
        }
    }

	void Update()
	{
		
		if (!isLocalPlayer)
		{
			return;
		}


        // non vr player input here
        var x = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        var z = Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;

        transform.Translate(new Vector3(x,0,z));

        mouseLook.LookRotation (transform, Camera.main.transform);



        // For flight.
        //transform.rotation = Camera.main.transform.rotation;

        // common input here
        if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}
	}

	// This [Command] code is called on the Client …
	// … but it is run on the Server!
	[Command]
	protected void CmdFire()
	{
        // There are issues with the client player as to where the bullet is spawning. 
        // bulletSpawn may not be read correctly? bulletSpawn may not be sent through the network correctly?
        // Try reworking the original prefab
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

		// Spawn the bullet on the Clients
		NetworkServer.Spawn(bullet);

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}
}