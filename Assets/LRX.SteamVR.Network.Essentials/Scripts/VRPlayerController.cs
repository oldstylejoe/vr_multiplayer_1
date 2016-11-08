/* Original Code from SteamVR Essentials
 * Purpose: Allow VR Player to control character with Vive
 * 
 * Modifications made by Mohammad Alam
 *  - Spome Old Child-Parent Combinations have been scrapped for new DamageImage mechanic
 *      - This means old prefabs for the Player are no longer compatible
 *  - There are frequent Get calls that have been optimized by adding variables
 *  - Datalogger Connection added
 *      - Sends Player Name and Transforms for DataLogging
 */

// Note: When incorporating new character models here, either edit code accordingly,
// specifically with the GetChild() calls or follow the current player's prefab style.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System;

public class VRPlayerController : NetworkBehaviour
{
    // Original Variables
    public GameObject vrCameraRig;
	public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;

    private GameObject vrCameraRigInstance;

    // New Variables
    // Component Caching
    private DataLogger datalogger;

    public override void OnStartLocalPlayer ()
	{
		if (!isClient)
			return;

        // delete main camera
        DestroyImmediate (Camera.main.gameObject);

		// create camera rig and attach player model to it
		vrCameraRigInstance = (GameObject)Instantiate (
			vrCameraRig,
			transform.position,
			transform.rotation);

        // Check if there is a body object as well
		Transform bodyOfVrPlayer = transform.FindChild ("VRPlayerBody");
		if (bodyOfVrPlayer != null)
			bodyOfVrPlayer.parent = null;

        // Initialize VR Head
		GameObject head = vrCameraRigInstance.GetComponentInChildren<SteamVR_Camera> ().gameObject;
		transform.parent = head.transform;
        transform.localPosition = new Vector3(0f, -0.03f, -0.06f);

        // Initialize DamageImage
        // Note: Modification was made to take out the HP label and add in a new damage image canvas.
        //       This means the object has a different array of children. The original prefab will no
        //       longer work with this code. Only the new modified one will.
        Transform HUDCanv = transform.GetChild(0);
        HUDCanv.GetComponent<Canvas>().worldCamera = Camera.main;
        HUDCanv.GetComponent<Canvas>().planeDistance = .1f;
        GetComponent<Health>().damageImage = HUDCanv.GetChild(0).GetComponent<Image>();

        // Check for DataLogger Object
        GameObject dataloggerTest = GameObject.FindGameObjectWithTag("DataLogger");

        if (dataloggerTest)
        {
            datalogger = dataloggerTest.GetComponent<DataLogger>();
            datalogger.Player = head.transform;
        }

        TryDetectControllers ();
	}

	void TryDetectControllers ()
	{
		var controllers = vrCameraRigInstance.GetComponentsInChildren<SteamVR_TrackedObject> ();
        if (controllers != null && controllers.Length == 2 && controllers[0] != null && controllers[1] != null)
        {
            if (datalogger)
            {
                datalogger.LeftHand = vrCameraRigInstance.GetComponent<SteamVR_ControllerManager>().left.transform;
                datalogger.RightHand = vrCameraRigInstance.GetComponent<SteamVR_ControllerManager>().right.transform;
            }
            CmdSpawnHands(netId);
        }
        else
        {
            Invoke("TryDetectControllers", 2f);
        }
	}

	[Command]
	void CmdSpawnHands(NetworkInstanceId playerId)
	{
        // instantiate controllers
        // tell the server, to spawn two new networked controller model prefabs on all clients
        // give the local player authority over the newly created controller models
        GameObject leftHand = Instantiate(leftHandPrefab);
		GameObject rightHand = Instantiate(rightHandPrefab);

		var leftVRHand = leftHand.GetComponent<NetworkVRHands> ();
		var rightVRHand = rightHand.GetComponent<NetworkVRHands> ();

		leftVRHand.side = HandSide.Left;
		rightVRHand.side = HandSide.Right;
        leftVRHand.ownerId = playerId;
		rightVRHand.ownerId = playerId;

		NetworkServer.SpawnWithClientAuthority (leftHand, base.connectionToClient);
		NetworkServer.SpawnWithClientAuthority (rightHand, base.connectionToClient);
    }

	[Command]
	public void CmdGrab(NetworkInstanceId objectId, NetworkInstanceId controllerId)
	{
		var iObject = NetworkServer.FindLocalObject (objectId);
		var networkIdentity = iObject.GetComponent<NetworkIdentity> ();
        networkIdentity.AssignClientAuthority(connectionToClient);

        var interactableObject = iObject.GetComponent<InteractableObject>();
        interactableObject.RpcAttachToHand (controllerId);    // client-side
        var hand = NetworkServer.FindLocalObject(controllerId);
        interactableObject.AttachToHand(hand);    // server-side
    }

	[Command]
	public void CmdDrop(NetworkInstanceId objectId, Vector3 currentHolderVelocity)
	{
		var iObject = NetworkServer.FindLocalObject (objectId);
		var networkIdentity = iObject.GetComponent<NetworkIdentity> ();
        networkIdentity.RemoveClientAuthority(connectionToClient);
        
        var interactableObject = iObject.GetComponent<InteractableObject>();
        interactableObject.RpcDetachFromHand(currentHolderVelocity); // client-side
        interactableObject.DetachFromHand(currentHolderVelocity); // server-side
    }
}
