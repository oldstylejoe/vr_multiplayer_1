using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EndGame : NetworkBehaviour {
	
	// Update is called once per frame
	void FixedUpdate () {
	    
	}

    void OnTriggerEnter (Collider col)
    {
        Debug.Log(col.gameObject.name);
    }
}
