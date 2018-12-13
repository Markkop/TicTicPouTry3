using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class Atributos : NetworkBehaviour {

	//SyncVar parece nao estar funcionando, pelo menos para newName.
	//Esta sendo necessario chamar RpcChangePlayerName() para que os
	//outros clients saibam da mudanca de newName no server.

	//[SyncVar (hook="OnNameChange")] public string newName = "";
	public string newName = "";

	[SyncVar] public bool vaiAtirar = false;
	[SyncVar] public bool vaiDefender = false;
	[SyncVar] public bool estaDefendendo;
	[SyncVar] public bool vaiRecarregar = false;
	[SyncVar] public bool ready = false;
	[SyncVar] public bool levouTiro = false;

	[SyncVar] public int vidas = 2;
	[SyncVar] public int balas = 0;
	public int maxBalas = 1;	

	//public GameObject[] playersArray;
	public List<GameObject> playersArray = new List<GameObject>();

	public GameObject alvo;
	public GameObject mortoPor; 
	
	public GameObject _Manager;
	public GameObject[] cameras;
	public GameObject playerCamera;
	public GameObject playerCanvas;
	public GameObject alvosPanel;
	public GameObject toggleGroup;
	public Toggle atiraButton;
	public Animator anim;

	public bool playerVencedor = false;
	public bool playerMorto = false;
	private bool firstSpawn = false;

	// Use this for initialization
	void Start () {

		if(this.GetComponent<botIA>() == null)
		{
			vidas = Settings.startingVidas;
			balas = Settings.startingBalas;
		}
		
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");



		//Vira a camera tambem 
		if(isLocalPlayer)
		{
			playerCamera.GetComponent<Transform>().LookAt(new Vector3(0f,1.5f,0f));	
		}

		//Caso o objeto seja spawnado sem autoridade. Usar para debugging, mas de preferencia nao
		//usar para evitar conflito.
		if(!hasAuthority)
		{
			//CmdAttachAuthority();
		}

		//Ativa a camera e o canvas apenas para o jogador local
		//Caso contrario, o mesmo painel aparece para todos de forma sobreposta
		if (isLocalPlayer)
		{
			//cameras[0].SetActive(false);
			playerCamera.SetActive(true);
			playerCanvas.SetActive(true);
		}
		else
		{	//Se um bot tenta isso, aparece mensagem de erro pela falta de referencia;
			//entao soh roda para quem nao tiver o script botIA (ou seja; players)
			if(this.GetComponent<botIA>() == null)
			{
				playerCamera.SetActive(false);
				playerCanvas.SetActive(false);
			}
		}

		//Pega o Manager da cena
		_Manager = GameObject.FindWithTag("Manager");
		_Manager.GetComponent<_Manager>().playersArray.Add(gameObject);

		if(isServer && this.GetComponent<botIA>() == null)
		{
			Debug.Log("Eh server e eh player real");
			if(_Manager.GetComponent<_Manager>().addBotPanel != null)
			{
				_Manager.GetComponent<_Manager>().addBotPanel.SetActive(true);	
			}
			
		}

	}
	
	// Update is called once per frame
	void Update () { 

		//Mantem a lista de player atualizada do _Manager
		if(playersArray != _Manager.GetComponent<_Manager>().playersArray)
		{
			playersArray = _Manager.GetComponent<_Manager>().playersArray;	
		}
			
	}

	//Funcao para Debug
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
		ready = boleano;
	}

	public void EscolheAlvo(int player)
	{

		for(int i = 0; i < playersArray.Count; i++) 
		{	//Se o play
			if(player == i)
			{
				alvo = playersArray[i]; //Da o gameObject indexado de acordo com o botao clicado
			}
		} 
		
	}

}
