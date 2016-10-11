using UnityEngine;
using System.Collections;
using VRTK;

public class ControllerListener : MonoBehaviour {

    public GameObject controller;
    public Rigidbody bullet;

    public float speed = 0.01f;

	// Use this for initialization
	void Start () {
        controller.GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(OnTrigger);
	}

    private void OnTrigger(object sender, ControllerInteractionEventArgs e) {
        //Debug.Log("Trigger pressed");

        Rigidbody clone = Instantiate<Rigidbody>(bullet);
        clone.transform.position = transform.position;
        clone.velocity = transform.forward * speed;
    }
	
	//// Update is called once per frame
	//void Update () {
	//
	//}
}
