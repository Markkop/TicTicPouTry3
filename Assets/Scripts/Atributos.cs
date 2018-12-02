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
	[SyncVar (hook="OnNameChange")] public string newName = "";

	public Animator anim;
	public Rigidbody rb;

	[SyncVar] public bool vaiAtirar = false;
	[SyncVar] public bool vaiDefender = false;
	[SyncVar] public bool estaDefendendo;
	[SyncVar] public bool vaiRecarregar = false;
	[SyncVar] public bool ready = false;
	[SyncVar] public bool levouTiro = false;

	[SyncVar] public int vidas = 2;
	[SyncVar] public int balas = 0;
	public int maxBalas = 1;	

	public GameObject[] playersArray;
	public GameObject alvo;
	public GameObject mortoPor; 
	public GameObject alvosPanel;

	public GameObject[] cameras;
	public GameObject playerModel;
	public GameObject playerCamera;
	public GameObject playerCanvas;
	public GameObject someInfoCanvas;
	public GameObject toggleGroup;
	public Toggle atiraButton;

	public bool playerVencedor = false;
	public bool playerMorto = false;


	//[SyncVar (hook="WhenAllReady")] public bool allReady;
	[SyncVar] public bool allReady;




	// Use this for initialization
	void Start () {
		
		cameras = GameObject.FindGameObjectsWithTag("MainCamera");

		//Olha para o centro da arena (hardcoded)
		playerModel.GetComponent<Transform>().LookAt(new Vector3(0f,1.5f,0f));

		//Vira a camera tambem
		if(isLocalPlayer)
		{
			playerCamera.GetComponent<Transform>().LookAt(new Vector3(0f,1.5f,0f));	
		}
		

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

	}
	
	// Update is called once per frame
	void Update () { 


		//Atualiza a lista de players
		playersArray = GameObject.FindGameObjectsWithTag("Player");

		/*//Funcao para forcar a mudanca de nome. Obsoleto for now.
		if(this.name != newName && newName != "")
		{
			//this.name = newName;
		}*/

		//Chama quando todos estiverem prontos. Serve apenas pra desativar os Toggles por enquanto
		if(allReady == true)
			WhenAllReady();

		//Ao morrer
		if(vidas == 0)
			CmdOnDeath();


		//Quando o player for vitorioso
		if(isLocalPlayer && this.GetComponent<botIA>() == null)
		{
			int mortos = 0;
			foreach(GameObject go in playersArray)
			{
				if(go.name == "Morto")
				{
					mortos++;
				}
			}
			if(mortos == playersArray.Length - 1 && this.vidas > 0)
			{
				//OnVictory();
			}
			else
			{
				playerVencedor = false;
			}
		}

		//Idle
		if(isLocalPlayer && this.GetComponent<botIA>() == null)
		{
			if(playerMorto == false && playerVencedor == false)
			{
				anim.Play("WAIT00");
			}
		}


		//Textos para debug. 
		/*if(isLocalPlayer && this.GetComponent<botIA>() == null)
		{
			someInfoCanvas.GetComponent<Text>().text =
			"hasAuthority == "+hasAuthority+"\n"+
			"isLocalPlayer == "+isLocalPlayer+"\n"+
			"isClient == "+isLocalPlayer+"\n"+
			"isServer == "+isServer;
		}*/

		//Debug de coisas
		if( Input.GetKeyDown("3") )
		{
			if(newName.Contains("Jogador"))
			{
				Debug.Log("Contem");
			}
		}

	}

	//Funcao chamada quando a SyncVar newName eh alterada (pelo _Manager)
	void OnNameChange(string newNamez)
	{
		Debug.Log("OnNameChange: mudando nome local de "+this.name+" para "+newNamez);
		newName = newNamez;

		if(isLocalPlayer){
		foreach(Transform child in alvosPanel.transform)
		{
			if(child.name == this.name)
			{
				child.GetComponent<Text>().text = newNamez;
			}
		}}

		gameObject.name = newNamez;
	}

	[Command]
	void CmdOnDeath() {

		if(playerMorto == true)
		{
			ready = true;
			return;
		}

		//Se for player
		if(isLocalPlayer)
		{
			//Muda de camera
			playerCamera.SetActive(false);
			cameras[0].active = true;

			//Desativa painel
			playerCanvas.SetActive(false);

			anim.Play("DAMAGED00");
		}


		playerMorto = true;

		

		//Workaround pra que o player/bot seja empurrado pra tras
		//Nao entendi o porque de ter que usar diferentes formas de forca pra cada caso,
		//Talvez por playerModelo ser child do Jogador 0 e a forca ser relativa a ele e
		//nao a si mesmo
		if(this.GetComponent<botIA>() == null)
		{
			playerModel.GetComponent<Transform>().LookAt(mortoPor.GetComponent<Transform>().position);
			playerModel.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * -4000);
		}
		else
		{	
			this.GetComponent<Transform>().LookAt(mortoPor.GetComponent<Transform>().position);
			this.GetComponent<Rigidbody>().AddForce(transform.forward * -4000);
		}

		//this.GetComponent<Rigidbody>().AddForce(transform.forward * -4000);
		//Debug.Log(this.name+ " esta morto");




	}

	void OnVictory() {

		playerVencedor = true;
		if(isLocalPlayer && this.GetComponent<botIA>() == null)
		{	
			anim.Play("WIN00");
		}

	}

	//Funcao para ser chamada quando todos os estiverem prontos
	void WhenAllReady() {

		if(this.GetComponent<botIA>() == null) //Faz os seguintes comandos apenas para quem for player
		{		
			//Desativa os toggles do jogador
			GameObject[] toggles = GameObject.FindGameObjectsWithTag("PlayerToggle");
			foreach (GameObject tog in toggles)
			{
				tog.GetComponent<Toggle>().isOn = false;
			}
	
			//Desativa o painel de alvos e os botoes de alvo
			//this.alvosPanel.SetActive (false);
			//this.GetComponent<ButtonCreator>().Destroi(); //Destroi os botoes de alvos	
		}
		//Termina resetando
		allReady = false;	
	}

	//Metodo antigo para mudanca de nome do player. Ate que funcionava, mas prefero
	//que o _Manager faça isso.
	/*[Command]
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
    */

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
		ready = boleano;
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
