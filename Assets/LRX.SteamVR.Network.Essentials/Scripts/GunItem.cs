/* Original Code from SteamVR Essentials
 * Allows Guns or, in this case, GunHands to fire when used and held in VR using controller triggers
 * 
 * Modifications made by Mohammad Alam
 *  - Changed method for gunfire.
 */

// Gun Shot Sound Courtesy of Freesound.org: http://freesound.org/people/Brokenphono/sounds/344143/

using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class GunItem : NetworkBehaviour, IUsable {
    public GameObject projectilePrefab;
    private AudioSource audioSrc;
    private Transform barrel;
    public float speed = 6f;

    void Start()
    {
        barrel = transform.FindChild("Barrel");
        audioSrc = GetComponent<AudioSource>();
    }

    [ClientRpc]
    private void RpcPlayGunShot(Vector3 soundStart)
    {
        // Play Sound over network 
        if (audioSrc)
            audioSrc.Play();
    }

    public void StartUsing(NetworkInstanceId handId)
    {
        // Spawn Bullet
        var projectile = (GameObject)Instantiate(projectilePrefab, barrel.position, barrel.rotation);
        projectile.GetComponent<Rigidbody>().velocity = barrel.up * speed;

        // Play Sound
        RpcPlayGunShot(barrel.position);

        NetworkServer.Spawn(projectile);
    }
	public void StopUsing(NetworkInstanceId handId)
	{
	}
}
