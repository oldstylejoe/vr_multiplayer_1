using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    // Slight modifications made to Steam VR Network Essentials code for aim clamping and correct movement

	public GameObject bulletPrefab;
	private MouseLook mouseLook;
	public Transform bulletSpawn;
    public float speed;

    public override void OnStartLocalPlayer()
	{
        transform.position = new Vector3(transform.position.x + -1f, transform.position.y + .8f, transform.position.z);
        transform.rotation = Quaternion.Euler(0,90,0);
		GetComponent<Renderer>().material.color = Color.blue;

        // attach camera to player.. 3rd person view..
        Camera.main.transform.parent = transform;
		Camera.main.transform.localPosition = new Vector3 (0, .5f, 0);
		Camera.main.transform.localRotation = Quaternion.Euler (6.31f, 0, 0);

        transform.GetChild(3).parent = Camera.main.transform;

        mouseLook = new MouseLook ();
		mouseLook.Init (transform, Camera.main.transform);
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