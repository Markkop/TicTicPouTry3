using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class keepFacingCamera : NetworkBehaviour {

	public GameObject player;
	//public GameObject[] players;
	public List<GameObject> players = new List<GameObject>();
	public GameObject[] cameras;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//players = GameObject.FindGameObjectsWithTag("Player");
		players = player.GetComponent<Atributos>().playersArray;

		foreach(GameObject player in players)
		{
			//Se o jogador for ele mesmo (localPlayer)
			if(player.GetComponent<NetworkIdentity>().isLocalPlayer == true)
			{
				if(player.GetComponent<Atributos>().playerCamera.active == true)
				{
					transform.LookAt(player.GetComponent<Atributos>().playerCamera.transform);	
				}
				else
				{
					transform.LookAt(Camera.main.transform);	
				}
				
			}
		}
		

		
	}


}
