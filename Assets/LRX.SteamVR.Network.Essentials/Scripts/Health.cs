using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour 
{
    // Script from SteamVR Network Essentials

	public const int maxHealth = 100;
	public bool destroyOnDeath;
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);

	private NetworkStartPosition[] spawnPoints;
    private bool damaged;

	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = maxHealth;

	void Start ()
	{
		if (isLocalPlayer)
		{
			spawnPoints = FindObjectsOfType<NetworkStartPosition>();
            damaged = false;
            damageImage = transform.parent.GetChild(0).GetChild(0).GetComponent<Image>();
		}
	}

    void Update ()
    {
        if (damageImage)
        {
            if (damaged)
            {
                damageImage.color = flashColor;
            }
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
        }
        damaged = false;
    }

	public void TakeDamage(int amount)
	{
		if (!isServer)
		{
			return;
		}

        currentHealth -= amount;

        damaged = true;

		if (currentHealth <= 0)
		{
			if (destroyOnDeath) {
				Destroy (gameObject);
			} else {
				currentHealth = maxHealth;
                if(GetComponent<PlayerController>())
				    RpcRespawn ();
			}
		}
	}

	void OnChangeHealth (int currentHealth)
	{
        if(GetComponentInChildren<TextMesh>())
		    GetComponentInChildren<TextMesh> ().text = currentHealth.ToString();
	}

	[ClientRpc]
	void RpcRespawn()
	{
		if (isLocalPlayer) {
			// Set the spawn point to origin as a default value
			Vector3 spawnPoint = Vector3.zero;

			// If there is a spawn point array and the array is not empty, pick one at random
			if (spawnPoints != null && spawnPoints.Length > 0)
			{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
			}

			// Set the player’s position to the chosen spawn point
			//transform.p
			Transform ancestor = GetAncestor(transform);
			ancestor.position = spawnPoint;
		}
	}

	Transform GetAncestor(Transform child)
	{
		Transform currentObject = child;
		while (currentObject.parent != null) {
			currentObject = currentObject.parent;
		}
		return currentObject;
	}
}