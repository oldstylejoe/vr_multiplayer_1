/* Original Code from SteamVR Essentials
 * Allows Guns or, in this case, GunHands to fire when used and held in VR using controller triggers
 * 
 * Modifications made by Mohammad Alam
 *  - Changed method for gunfire.
 */

// Gun Shot Sound Courtesy of Freesound.org: http://freesound.org/people/Brokenphono/sounds/344143/

using UnityEngine;
using UnityEngine.Networking;

public class GunItem : NetworkBehaviour, IUsable {
    public GameObject projectilePrefab;
    public AudioClip gunShotSound;
    private Transform barrel;
    public float speed = 6f;
    public float soundVol = 0.01f;

    void Start()
    {
        barrel = transform.FindChild("Barrel");
    }

	public void StartUsing(NetworkInstanceId handId)
    {
        // Spawn Bullet
        var projectile = (GameObject)Instantiate(projectilePrefab, barrel.position, barrel.rotation);
        projectile.GetComponent<Rigidbody>().velocity = barrel.up * speed;

        // Play Sound
        if(gunShotSound)
            AudioSource.PlayClipAtPoint(gunShotSound,barrel.position, soundVol);

        NetworkServer.Spawn(projectile);
    }
	public void StopUsing(NetworkInstanceId handId)
	{
	}
}
