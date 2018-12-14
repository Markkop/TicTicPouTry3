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

	// Lista de bots
	// 1 - Defensor
	// 2 - Sanguinario
	// 3 - Oraculo
	// 4 - AlvoEsperto


	[Command]
	public void CmdSpawnBot(int bot)
	{
		if(!isServer)
		{
			Debug.Log(this.name+" tentou invocar um bot, mas nao eh server");
			return;
		}


		GameObject go = (GameObject)Instantiate(botPrefab);
	
		//Arruma a altura do spawn para que o objeto seja transportado para a base
		//do spawn point e nao ao centro.
		
		//var spawn = NetworkManager.singleton.GetStartPosition();
		//var spawn = new Vector3(0, 0, 0);
		
		//float altura = go.GetComponent<Collider>().bounds.max[1];
		//altura = altura/2;
		//go.GetComponent<Transform>().position = spawn + new Vector3(0,altura,0); //spawn.position
		NetworkServer.Spawn(go);
		
		switch(bot){
			case 1:
			go.GetComponent<botIA>().botDefensor = true;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
			RpcSpawnBot(go, Color.blue); //Avisa outros clients dessa mudança
			break;

			case 2:
			go.GetComponent<botIA>().botSanguinario = true;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
			RpcSpawnBot(go, Color.red);
			break;

			case 3:
			go.GetComponent<botIA>().botOraculo = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
			RpcSpawnBot(go, Color.white);
			break;

			case 4:
			go.GetComponent<botIA>().botAlvoEsperto = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
			RpcSpawnBot(go, Color.green);
			break;

			case 5:
			go.GetComponent<botIA>().botRDRA = true;			
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.grey;
			RpcSpawnBot(go, Color.grey);
			break;

			case 6:
			go.GetComponent<botIA>().botMagoSanguinario = true;			
			go.GetComponent<Atributos>().classe = 1;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.magenta;
			RpcSpawnBot(go, Color.magenta);
			break;

			case 7:
			go.GetComponent<botIA>().botSamuraiMedroso = true;			
			go.GetComponent<Atributos>().classe = 2;
			go.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
			RpcSpawnBot(go, Color.white);
			break;
		}
		


	}

	[ClientRpc]
	public void RpcSpawnBot(GameObject bot, Color cor)
	{
		bot.GetComponent<Transform>().Find("Beta_Surface").gameObject.GetComponent<SkinnedMeshRenderer>().material.color = cor;
	}


}

