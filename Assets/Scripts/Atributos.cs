using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class Atributos : NetworkBehaviour {

	public string newName;
	[SyncVar] public bool vaiAtirar = false;
	[SyncVar] public bool vaiDefender = false;
	[SyncVar] public bool estaDefendendo;
	[SyncVar] public bool vaiRecarregar = false;
	[SyncVar] public bool ready = false;
	[SyncVar] public bool levouTiro = false;

	[SyncVar] public int vidas = 2;
	[SyncVar] public int balas = 0;
	public int maxBalas = 1;	

	public GameObject alvo;
	public GameObject alvosPanel;

	public GameObject playerCamera;
	public GameObject playerCanvas;
	public GameObject toggleGroup;



	// Use this for initialization
	void Start () {
		
		//Olha para o centro da arena (hardcoded)
		this.GetComponent<Transform>().LookAt(new Vector3(0f,0f,0f));

		//Muda o nome
		CmdChangeName();

		//Caso o objeto seja spawnado sem autoridade
		if(!hasAuthority)
		{
			CmdAttachAuthority();
		}

		if (isLocalPlayer)
		{
			//playerCamera.SetActive(true);
			playerCanvas.SetActive(true);
		}
		else
		{
			if(this.GetComponent<botIA>() == null) //Se nao for um bot (for um player)
			{
				//playerCamera.SetActive(false);
				playerCanvas.SetActive(false);
			}
		}

	}
	
	// Update is called once per frame
	void Update () { }

	[Command]
	void CmdChangeName()
	{
		GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
		int i = 0;
		foreach (GameObject go in playersArray)
		{
			if (go.name == this.name)
			{
				newName = "Jogador "+i;
				Debug.Log(go.name+" muda nome para "+newName);
				this.name = newName;
				RpcChangePlayerName(newName);
			}
			else
			{
				i++;
			}
		}
	}

	[ClientRpc]
    void RpcChangePlayerName(string n)
    {
        this.name = n;
    }

	[Command]
    void CmdAttachAuthority()
    {
		//Da a autoridade sob o objeto para quem conectou com este client
		this.GetComponent<NetworkIdentity>().AssignClientAuthority ( connectionToClient );
		Debug.Log(this.name+" recebeu autoridade de "+connectionToClient);
    }


    [Command]
	public void CmdQuerDefender2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = true;
			vaiAtirar = false;
			vaiRecarregar = false;	
		}
		else
		{
			vaiDefender = false;
		}
		
		//Debug.Log(this.name +" defende?: "+boleano);
	}

	[Command]
	public void CmdQuerAtirar2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = true;
			vaiRecarregar = false;
			this.alvosPanel.gameObject.SetActive (true);
		}
		else
		{
			vaiAtirar = false;
		}
		//Debug.Log(this.name +" atira?: "+boleano);
	}

    [Command]
	public void CmdQuerRecarregar2(bool boleano)
	{
		if (boleano == true)
		{
			vaiDefender = false;
			vaiAtirar = false;
			vaiRecarregar = true;	
		}
		else
		{
			vaiRecarregar = false;
		}
		//Debug.Log(this.name +" recarrega?: "+boleano);
	}


	[ClientRpc]
	void RpcMudaDefesa(bool boleano)
	{
		Debug.Log("Server: pedido de mudanca de DEFESA para "+boleano);
		this.vaiDefender = boleano;
	}



	[Command]
	public void CmdIsReady(bool boleano)
	{
		if(boleano == true)
		{
			//A fazer: desativar toggle das acoes tb
			//this.alvosPanel.gameObject.SetActive (false);
			ready = true;
		}
		else
		{
			ready = false;
		}
	}

	public void EscolheAlvo(int player)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); //Botas os players num array

		for(int i = 0; i < players.Length; i++) 
		{	//Se o play
			if(player == i)
			{
				alvo = players[i]; //Da o gameObject indexado de acordo com o botao clicado
			}
		} 
		
	}

}
