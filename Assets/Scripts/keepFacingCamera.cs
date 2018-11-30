using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class keepFacingCamera : NetworkBehaviour {

	public GameObject[] players;
	public GameObject[] cameras;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// players = GameObject.FindGameObjectsWithTag("Player");
		// foreach(GameObject go in players)
		// {
		// 	if (go == isLocalPlayer)
		// 	{
		// 		transform.LookAt(go.transform);		
		// 	}
		// }
		
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");
		//players = GameObject.FindGameObjectsWithTag("Player");
		transform.LookAt(cameras[1].transform);		

		
	}


}
