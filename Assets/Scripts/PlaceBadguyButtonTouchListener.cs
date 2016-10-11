using UnityEngine;
using System.Collections;
using VRTK;
using UnityEngine.Networking;

public class PlaceBadguyButtonTouchListener : NetworkBehaviour {

    public BadGuyManager bgm;
    public GameObject controller;

    // Use this for initialization
    void Start () {
        controller.GetComponent<VRTK_InteractTouch>().ControllerTouchInteractableObject += new ObjectInteractEventHandler(CmdTouchButton);
	}

    [Command]
    void CmdTouchButton(object sender, ObjectInteractEventArgs e)
    {
        Debug.Log("Touch the button");
    }
	
}
