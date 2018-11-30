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

	}



	[Command]
	public void CmdSpawnBot()
	{
		if(!isServer)
		{
			Debug.Log(this.name+" tentou invocar um bot, mas nao eh server");
			return;
		}

		GameObject go = (GameObject)Instantiate(botPrefab);
		var spawn = NetworkManager.singleton.GetStartPosition();
	
		//Arruma a altura do spawn para que o objeto seja transportado para a base
		//do spawn point e nao ao centro.
		float altura = go.GetComponent<Collider>().bounds.max[1];
		altura = altura/2;
		go.GetComponent<Transform>().position = spawn.position + new Vector3(0,altura,0);
		NetworkServer.Spawn(go);
	}

	[ClientRpc]
	public void RpcSpawnBot()
	{

	}
}

