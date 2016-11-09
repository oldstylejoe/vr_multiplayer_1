using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AmbienceAudio : NetworkBehaviour {

	// Use this for initialization
	public override void OnStartLocalPlayer () {
        CmdReadyAmbience();
	}

    [Command]
    private void CmdReadyAmbience()
    {
        RpcPlayAmbience();
    }

    [ClientRpc]
    private void RpcPlayAmbience()
    {
        GetComponent<AudioSource>().Play();
    }
}
