using UnityEngine;
using UnityEngine.Networking;


public class PlayerController : NetworkBehaviour
{
	public GameObject bulletPrefab;
	private MouseLook mouseLook;
	public Transform bulletSpawn;
    public float speed;

    private Rigidbody rb;

	public override void OnStartLocalPlayer()
	{
        transform.position = new Vector3(transform.position.x + -1f, transform.position.y + .8f, transform.position.z);
        transform.rotation = Quaternion.Euler(0,90,0);
		GetComponent<Renderer>().material.color = Color.blue;
        rb = GetComponent<Rigidbody>();

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
        var z = Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;
        var x = Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;

        transform.localPosition += new Vector3(x,0,z);

        /*
		var x = Input.GetAxis ("Horizontal") * transform.right;
		var z = Input.GetAxis ("Vertical") * transform.forward;

        Vector3 velocity = (x + z) * speed;
        if (velocity != Vector3.zero)
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        else
            rb.velocity = Vector3.zero;
            */

        mouseLook.LookRotation (transform, Camera.main.transform);

        transform.rotation = Camera.main.transform.rotation;

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