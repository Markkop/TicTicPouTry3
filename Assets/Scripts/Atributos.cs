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
	public string newName;

	public Animator anim;

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

	public GameObject[] playersArray;

	[SyncVar] public bool allReady;

	// Use this for initialization
	void Start () {
		
		//Olha para o centro da arena (hardcoded)
		this.GetComponent<Transform>().LookAt(new Vector3(0f,0f,0f));

		//Importa o animator (se pa da pra fazer direto pelo editor)
		//anim = GetComponent<Animator>();

		//Tentei chamar a funcao CmdChangeName() no Start(), mas clients nao eram ouvidos sobre
		//a mudanca, entao passei a chamar no Update(). Fica a dica
		//CmdChangeName();

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

	}
	
	// Update is called once per frame
	void Update () { 

		//Atualiza a lista de players
		playersArray = GameObject.FindGameObjectsWithTag("Player");

		//Se ainda nao tiver nome (default), muda nome
		if(newName == "")
			CmdChangeName();

		//Chama quando todos estiverem prontos. Serve apenas pra desativar os Toggles por enquanto
		if(allReady == true)
			WhenAllReady();

		//Ao morrer
		if(vidas == 0)
			OnDeath();

		//Quando o player for vitorioso
		if(playersArray.Length == 1 && vidas > 0)
			OnVictory();

		if( Input.GetKeyDown("1") )
			Debug.Log("KeyPressed");
			anim.Play("WIN00");

	}

	void OnDeath() {

		//Se for jogador local e nao bot
		if(isLocalPlayer && this.GetComponent<botIA>() == null)
		{
			GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
			cameras[1].GetComponent<Camera>().enabled = false;
			cameras[0].GetComponent<Camera>().enabled = true;

			playerCanvas.SetActive(false);
		}

	}

	void OnVictory() {

		//anim.Play("WIN00");
	}

	//Funcao para ser chamada quando todos os estiverem prontos
	void WhenAllReady() {

		//Desativa os toggles do jogador
		GameObject[] toggles = GameObject.FindGameObjectsWithTag("PlayerToggle");
		foreach (GameObject tog in toggles)
		{
			tog.GetComponent<Toggle>().isOn = false;
		}

		//Termina resetando
		allReady = false;		
	}


	[Command]
	void CmdChangeName()
	{
		int i = 0;
		string oldName = this.name;

		//Bota todos os jogadores num array, menos o player que tira a tag temporariamente
		this.tag = "Untagged";
		GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

		//Nome do jogador vira Jogador 0
		newName = "Jogador "+i;
		this.name = newName;

		foreach (GameObject go in playersArray)
		{
			if(go.name == newName) //Mas caso ja tenha algume chamado Jogador 0
			{
				i++; 
				newName = "Jogador "+i; //Vira Jogador 1
				this.name = newName;	
			}
				
		}

		Debug.Log(oldName+" muda nome para "+newName);

		//Avisa todos os clients dessa mudanca
		RpcChangePlayerName(newName);

		//Recupera a tag
		this.tag = "Player";
	}

	[ClientRpc]
    void RpcChangePlayerName(string n)
    {
    	Debug.Log("Avisando todos os clientes da mudanca de nome do "+n);
    	newName = n;
        this.name = n;
    }

	[Command]
    void CmdAttachAuthority()
    {
		//Da a autoridade sob o objeto para quem conectou com este client
		this.GetComponent<NetworkIdentity>().AssignClientAuthority ( connectionToClient );
		Debug.Log(this.name+" recebeu autoridade de "+connectionToClient);
    }

    bool CheckAllReady()
    {
		GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject go in playersArray)
		{
			if(go.GetComponent<Atributos>().ready == false)	
			{
				Debug.Log("CheckAllReady == False");
				return false;
			}
		}
		Debug.Log("CheckAllReady == True");
		return true;
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

		for(int i = 0; i < playersArray.Length; i++) 
		{	//Se o play
			if(player == i)
			{
				alvo = playersArray[i]; //Da o gameObject indexado de acordo com o botao clicado
			}
		} 
		
	}

}
