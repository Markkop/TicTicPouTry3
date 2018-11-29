using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class addBot : NetworkBehaviour {

	public GameObject botPrefab;


	// Use this for initialization
	void Start () { }
	
	// Update is called once per frame
	void Update () {

		if( Input.GetKeyDown(KeyCode.Space) )
		{
			Debug.Log(NetworkManager.singleton.GetStartPosition());
		}
	}



	[Command]
	public void CmdSpawnBot()
	{
		GameObject go = (GameObject)Instantiate(botPrefab);
		var spawn = NetworkManager.singleton.GetStartPosition();
		go.GetComponent<Transform>().position = spawn.position;
		NetworkServer.Spawn(go);
	}
}
